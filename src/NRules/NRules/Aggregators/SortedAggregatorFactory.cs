using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for projection aggregator.
    /// </summary>
    internal class SortedAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
        {
            SortDirection sortDirection;
            IAggregateExpression compiledSelector;

            var selector = element.ExpressionMap.FirstOrDefault(e => e.Name == "KeySelectorAscending");
            if (selector != null)
            {
                compiledSelector = compiledExpressions["KeySelectorAscending"];
                sortDirection = SortDirection.Ascending;
            }
            else
            {
                selector = element.ExpressionMap["KeySelectorDescending"];
                compiledSelector = compiledExpressions["KeySelectorDescending"];
                sortDirection = SortDirection.Descending;
            }

            var sourceType = element.Source.ValueType;
            var resultType = selector.Expression.ReturnType;

            var aggregatorType = typeof(SortedAggregator<,>).MakeGenericType(sourceType, resultType);
            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledSelector), Expression.Constant(sortDirection)));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}