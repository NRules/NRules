using System;
using System.Linq.Expressions;
using NRules.Extensibility;
using NRules.Rete;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules
{
    internal interface IRuleAction
    {
        Expression Expression { get; }
        object[] GetArguments(IExecutionContext executionContext, IActionContext actionContext);
        void Invoke(IExecutionContext executionContext, IActionContext actionContext, object[] arguments);
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

        public Expression Expression
        {
            get { return _expression; }
        }

        public object[] GetArguments(IExecutionContext executionContext, IActionContext actionContext)
        {
            var compiledRule = actionContext.CompiledRule;
            var activation = actionContext.Activation;
            var tuple = activation.Tuple;
            var tupleFactMap = activation.TupleFactMap;

            var args = new object[_compiledAction.ArrayArgumentCount];

            int index = tuple.Count - 1;
            var factIndexMap = _factIndexMap;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = factIndexMap[tupleFactMap[index]];
                IndexMap.SetElementAt(args, mappedIndex, fact.Object);
                index--;
            }

            index = 0;
            var dependencyIndexMap = _dependencyIndexMap;
            var dependencyResolver = executionContext.Session.DependencyResolver;
            var resolutionContext = new ResolutionContext(executionContext.Session, compiledRule.Definition);
            foreach (var dependency in compiledRule.Dependencies)
            {
                var mappedIndex = dependencyIndexMap[index];
                if (mappedIndex >= 0)
                {
                    var resolvedDependency = dependency.Factory(dependencyResolver, resolutionContext);
                    IndexMap.SetElementAt(args, mappedIndex, resolvedDependency);
                }
                index++;
            }

            return args;
        }

        public void Invoke(IExecutionContext executionContext, IActionContext actionContext, object[] arguments)
        {
            _compiledAction.Delegate.Invoke(actionContext, arguments);
        }
    }
}
