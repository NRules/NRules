using System;

namespace NRules.RuleModel.Aggregators
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

        public GroupByAggregatorFactory(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            _keySelector = keySelector;
            _elementSelector = elementSelector;
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TSource, TKey, TElement>(_keySelector, _elementSelector);
        }
    }
}