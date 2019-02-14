using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for collection aggregator.
    /// </summary>
    internal class CollectionAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
        {
            var sourceType = element.Source.ValueType;

            var ascendingSortSelector = element.ExpressionMap.Find("KeySelectorAscending");
            var descendingSortSelector = element.ExpressionMap.Find("KeySelectorDescending");
            if (ascendingSortSelector != null)
            {
                _factory = CreateSortedAggregatorFactory(sourceType, SortDirection.Ascending, ascendingSortSelector, compiledExpressions["KeySelectorAscending"]);
            }
            else if (descendingSortSelector != null)
            {
                _factory = CreateSortedAggregatorFactory(sourceType, SortDirection.Descending, descendingSortSelector, compiledExpressions["KeySelectorDescending"]);
            }
            else
            {
                var aggregatorType = typeof(CollectionAggregator<>).MakeGenericType(sourceType);
                var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                    Expression.New(aggregatorType));
                _factory = factoryExpression.Compile();
            }
        }

        Func<IAggregator> CreateSortedAggregatorFactory(Type sourceType, SortDirection sortDirection, NamedExpressionElement selector, IAggregateExpression compiledSelector)
        {
            var resultType = selector.Expression.ReturnType;
            var aggregatorType = typeof(SortedAggregator<,>).MakeGenericType(sourceType, resultType);

            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledSelector), Expression.Constant(sortDirection)));

            return factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}
