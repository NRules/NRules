using System;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        void Invoke(IExecutionContext executionContext, ActionContext actionContext, Tuple tuple, IndexMap tupleFactMap);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factIndexMap;
        private readonly IndexMap _dependencyIndexMap;
        private readonly FastDelegate<Action<object[]>> _compiledAction;

        public RuleAction(LambdaExpression expression, IndexMap factIndexMap, IndexMap dependencyIndexMap)
        {
            _expression = expression;
            _factIndexMap = factIndexMap;
            _dependencyIndexMap = dependencyIndexMap;
            _compiledAction = FastDelegate.Create<Action<object[]>>(expression);
        }

        public void Invoke(IExecutionContext executionContext, ActionContext actionContext, Tuple tuple, IndexMap tupleFactMap)
        {
            var args = new object[_compiledAction.ParameterCount];
            args[0] = actionContext;
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = _factIndexMap[tupleFactMap[index]];
                IndexMap.SetElementAt(ref args, mappedIndex, 1, fact.Object);
                index--;
            }

            index = 0;
            var dependencyResolver = executionContext.Session.DependencyResolver;
            foreach (var dependency in actionContext.CompiledRule.Dependencies)
            {
                var mappedIndex = _dependencyIndexMap[index];
                if (mappedIndex >= 0)
                {
                    var resolutionContext = new ResolutionContext(executionContext.Session, actionContext.Rule);
                    var resolvedDependency = dependency.Factory(dependencyResolver, resolutionContext);
                    IndexMap.SetElementAt(ref args, mappedIndex, 1, resolvedDependency);
                }
                index++;
            }

            try
            {
                _compiledAction.Delegate.Invoke(args);
            }
            catch (Exception e)
            {
                bool isHandled;
                executionContext.EventAggregator.RaiseActionFailed(executionContext.Session, e, _expression, tuple, out isHandled);
                if (!isHandled)
                {
                    throw new RuleActionEvaluationException("Failed to evaluate rule action", _expression.ToString(), e);
                }
            }
        }
    }
}