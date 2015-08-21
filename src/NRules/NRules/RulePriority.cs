using System;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRulePriority
    {
        int Invoke(IExecutionContext executionContext, ICompiledRule rule, Tuple tuple, IndexMap tupleFactMap);
    }

    internal class RulePriority : IRulePriority
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factIndexMap;
        private readonly IndexMap _dependencyIndexMap;
        private readonly FastDelegate<Func<object[], int>> _compiledExpression;

        public RulePriority(LambdaExpression expression, IndexMap factIndexMap, IndexMap dependencyIndexMap)
        {
            _expression = expression;
            _factIndexMap = factIndexMap;
            _dependencyIndexMap = dependencyIndexMap;
            _compiledExpression = FastDelegate.Create<Func<object[], int>>(expression);
        }

        public int Invoke(IExecutionContext executionContext, ICompiledRule rule, Tuple tuple, IndexMap tupleFactMap)
        {
            var args = new object[_compiledExpression.ParameterCount];
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = _factIndexMap[tupleFactMap[index]];
                IndexMap.SetElementAt(ref args, mappedIndex, 0, fact.Object);
                index--;
            }

            index = 0;
            var dependencyResolver = executionContext.Session.DependencyResolver;
            foreach (var dependency in rule.Dependencies)
            {
                var mappedIndex = _dependencyIndexMap[index];
                if (mappedIndex >= 0)
                {
                    var resolutionContext = new ResolutionContext(executionContext.Session, rule.Definition);
                    var resolvedDependency = dependency.Factory(dependencyResolver, resolutionContext);
                    IndexMap.SetElementAt(ref args, mappedIndex, 0, resolvedDependency);
                }
                index++;
            }

            try
            {
                return _compiledExpression.Delegate.Invoke(args);
            }
            catch (Exception e)
            {
                throw new RuleExpressionEvaluationException("Failed to evaluate priority expression", _expression.ToString(), e);
            }
        }
    }
}