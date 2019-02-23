using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for collection aggregator, including modifiers such as OrderBy.
    /// </summary>
    internal class CollectionAggregatorFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IEnumerable<IAggregateExpression> compiledExpressions)
        {
            var sourceType = element.Source.ValueType;

            var sortCriteriaKeySelectors = compiledExpressions.Where(x => x.Name == AggregateElement.KeySelectorAscendingName || x.Name == AggregateElement.KeySelectorDescendingName).ToArray();
            if (sortCriteriaKeySelectors.Any())
            {
                if (sortCriteriaKeySelectors.Length == 1)
                {
                    var keySelector = sortCriteriaKeySelectors[0];
                    _factory = CreateSingleKeySortedAggregatorFactory(sourceType, GetSortDirection(keySelector.Name), element.Expressions.FindSingleOrDefault(keySelector.Name), keySelector);
                }
                else
                {
                    var sortCriteria = compiledExpressions.Select(x => new SortCriteria(x, GetSortDirection(x.Name))).ToArray();
                    _factory = CreateMultiKeySortedAggregatorFactory(sourceType, sortCriteria);
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
            var resultType = selector.Expression.ReturnType;
            var aggregatorType = typeof(SortedAggregator<,>).MakeGenericType(sourceType, resultType);

            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledSelector), Expression.Constant(sortDirection)));

            return factoryExpression.Compile();
        }

        private static Func<IAggregator> CreateMultiKeySortedAggregatorFactory(Type sourceType, SortCriteria[] sortCriterias)
        {
            var aggregatorType = typeof(MultiKeySortedAggregator<>).MakeGenericType(sourceType);

            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(sortCriterias)));

            return factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }
}