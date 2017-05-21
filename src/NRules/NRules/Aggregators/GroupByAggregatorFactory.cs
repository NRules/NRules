using System;
using System.Linq.Expressions;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator factory for group by aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TElement">Type of elements to group.</typeparam>
    internal class GroupByAggregatorFactory<TSource, TKey, TElement> : IAggregatorFactory
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;

        public GroupByAggregatorFactory(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            _keySelector = keySelector.Compile();
            _elementSelector = elementSelector.Compile();
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TSource, TKey, TElement>(_keySelector, _elementSelector);
        }
    }
}