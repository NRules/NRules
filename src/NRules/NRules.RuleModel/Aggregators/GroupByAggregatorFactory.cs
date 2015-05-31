using System;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator factory for group by aggregator.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TValue">Type of values to group.</typeparam>
    internal class GroupByAggregatorFactory<TSource, TKey, TValue> : IAggregatorFactory
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TValue> _valueSelector;

        public GroupByAggregatorFactory(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            _keySelector = keySelector;
            _valueSelector = valueSelector;
        }

        public IAggregator Create()
        {
            return new GroupByAggregator<TSource, TKey, TValue>(_keySelector, _valueSelector);
        }
    }
}