using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        void Invoke(IExecutionContext executionContext, IContext actionContext, Tuple tuple, IndexMap tupleFactMap, IEnumerable<IRuleDependency> dependencies);
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

        public void Invoke(IExecutionContext context, IContext actionContext, Tuple tuple, IndexMap tupleFactMap, IEnumerable<IRuleDependency> dependencies)
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
            var dependencyResolver = context.Session.DependencyResolver;
            foreach (var dependency in dependencies)
            {
                var mappedIndex = _dependencyIndexMap[index];
                if (mappedIndex >= 0)
                {
                    var resolvedDependency = dependency.Factory(dependencyResolver);
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
                context.EventAggregator.RaiseActionFailed(context.Session, e, _expression, tuple, out isHandled);
                if (!isHandled)
                {
                    throw new RuleActionEvaluationException("Failed to evaluate rule action", _expression.ToString(), e);
                }
            }
        }
    }
}