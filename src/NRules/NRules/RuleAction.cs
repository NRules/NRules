using System;
using System.Linq.Expressions;
using NRules.Extensibility;
using NRules.Rete;
using NRules.RuleModel;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        void Invoke(IExecutionContext executionContext, IActionContext actionContext, Tuple tuple, IndexMap tupleFactMap);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factIndexMap;
        private readonly IndexMap _dependencyIndexMap;
        private readonly FastDelegate<Action<IContext, object[]>> _compiledAction;

        public RuleAction(LambdaExpression expression, IndexMap factIndexMap, IndexMap dependencyIndexMap)
        {
            _expression = expression;
            _factIndexMap = factIndexMap;
            _dependencyIndexMap = dependencyIndexMap;
            _compiledAction = FastDelegate.Action(expression);
        }

        public void Invoke(IExecutionContext executionContext, IActionContext actionContext, Tuple tuple, IndexMap tupleFactMap)
        {
            var args = new object[_compiledAction.ArrayArgumentCount];
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = _factIndexMap[tupleFactMap[index]];
                IndexMap.SetElementAt(args, mappedIndex, fact.Object);
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
                    IndexMap.SetElementAt(args, mappedIndex, resolvedDependency);
                }
                index++;
            }

            var invocation = new ActionInvocation(actionContext, tuple, args, _compiledAction.Delegate);
            try
            {
                var actionInterceptor = executionContext.Session.ActionInterceptor;
                if (actionInterceptor != null)
                {
                    actionInterceptor.Intercept(invocation);
                }
                else
                {
                    invocation.Invoke();
                }
            }
            catch (Exception e)
            {
                bool isHandled;
                executionContext.EventAggregator.RaiseActionFailed(executionContext.Session, actionContext.CompiledRule, e, _expression, tuple, out isHandled);
                if (!isHandled)
                {
                    throw new RuleActionEvaluationException("Failed to evaluate rule action",
                        actionContext.Rule.Name, _expression.ToString(), e);
                }
            }
        }
    }
}
