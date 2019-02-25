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
    /// Depending on supplied expressions this will create an ordered or an unordered fact aggregator.
    /// </summary>
    internal class CollectionAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IEnumerable<IAggregateExpression> compiledExpressions)
        {
            var sourceType = element.Source.ValueType;

            var sortConditions = compiledExpressions
                .Where(x => x.Name == AggregateElement.KeySelectorAscendingName || x.Name == AggregateElement.KeySelectorDescendingName)
                .Select(x => new SortCondition(x.Name, GetSortDirection(x.Name), x)).ToArray();
            if (sortConditions.Any())
            {
                if (sortConditions.Length == 1)
                {
                    var sortCondition = sortConditions[0];
                    var selectorElement = element.Expressions.FindSingleOrDefault(sortCondition.Name);
                    _factory = CreateSingleKeySortedAggregatorFactory(sourceType, sortCondition.Direction, selectorElement, sortCondition.KeySelector);
                }
                else
                {
                    _factory = CreateMultiKeySortedAggregatorFactory(sourceType, sortConditions);
                }
            }
            else
            {
                var aggregatorType = typeof(CollectionAggregator<>).MakeGenericType(sourceType);
                var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                    Expression.New(aggregatorType));
                _factory = factoryExpression.Compile();
            }
        }

        private static SortDirection GetSortDirection(string keySelectorName)
        {
            return keySelectorName == AggregateElement.KeySelectorAscendingName ? SortDirection.Ascending : SortDirection.Descending;
        }

        private static Func<IAggregator> CreateSingleKeySortedAggregatorFactory(Type sourceType, SortDirection sortDirection, NamedExpressionElement selector, IAggregateExpression compiledSelector)
        {
            var keyType = selector.Expression.ReturnType;
            var aggregatorType = typeof(SortedAggregator<,>).MakeGenericType(sourceType, keyType);

            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledSelector), Expression.Constant(sortDirection)));

            return factoryExpression.Compile();
        }

        private static Func<IAggregator> CreateMultiKeySortedAggregatorFactory(Type sourceType, SortCondition[] sortConditions)
        {
            var aggregatorType = typeof(MultiKeySortedAggregator<>).MakeGenericType(sourceType);

            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(sortConditions)));

            return factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}