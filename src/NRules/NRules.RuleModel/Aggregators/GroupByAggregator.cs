using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator that groups matching facts into collections of elements with the same key.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TValue">Type of values to group.</typeparam>
    internal class GroupByAggregator<TSource, TKey, TValue> : IAggregator
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TValue> _valueSelector;
        private readonly Dictionary<TKey, Grouping> _groups = new Dictionary<TKey, Grouping>();
        private readonly Dictionary<object, TKey> _sourceToKey = new Dictionary<object, TKey>(); 
        private readonly Dictionary<object, TValue> _sourceToValue = new Dictionary<object, TValue>(); 

        public GroupByAggregator(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            _keySelector = keySelector;
            _valueSelector = valueSelector;
        }

        public IEnumerable<AggregationResult> Initial()
        {
            return AggregationResult.Empty;
        }

        public IEnumerable<AggregationResult> Add(object fact)
        {
            var source = (TSource) fact;
            var key = _keySelector(source);
            var value = _valueSelector(source);
            _sourceToKey[fact] = key;
            _sourceToValue[fact] = value;
            return Add(key, value);
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            var source = (TSource)fact;
            var key = _keySelector(source);
            var value = _valueSelector(source);
            var oldKey = _sourceToKey[fact];
            var oldValue = _sourceToValue[fact];
            _sourceToKey[fact] = key;
            _sourceToValue[fact] = value;

            if (Equals(key, oldKey) && Equals(value, oldValue))
                return new[] {AggregationResult.Modified(_groups[key])};

            var result1 = Remove(oldKey, oldValue);
            var result2 = Add(key, value);
            return result1.Union(result2);
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            var oldKey = _sourceToKey[fact];
            var oldValue = _sourceToValue[fact];
            _sourceToKey.Remove(fact);
            _sourceToValue.Remove(fact);
            return Remove(oldKey, oldValue);
        }

        private IEnumerable<AggregationResult> Add(TKey key, TValue value)
        {
            Grouping group;
            if (!_groups.TryGetValue(key, out group))
            {
                group = new Grouping(key);
                _groups[key] = group;

                group.Add(value);
                return new[] { AggregationResult.Added(group) };
            }

            group.Add(value);
            return new[] { AggregationResult.Modified(group) };
        }
        
        private IEnumerable<AggregationResult> Remove(TKey key, TValue value)
        {
            var group = _groups[key];
            group.Remove(value);
            if (group.Count == 0)
            {
                _groups.Remove(key);
                return new[] { AggregationResult.Removed(group) };
            }

            return new[] { AggregationResult.Modified(group) };
        }

        public IEnumerable<object> Aggregates { get { return _groups.Values; } }

        private class Grouping : IGrouping<TKey, TValue>
        {
            private readonly TKey _key;
            private readonly List<TValue> _elements = new List<TValue>();

            public Grouping(TKey key)
            {
                _key = key;
            }

            public TKey Key { get { return _key; } }
            public int Count { get { return _elements.Count; } }

            public void Add(TValue value)
            {
                _elements.Add(value);
            }

            public void Remove(TValue value)
            {
                _elements.Remove(value);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return _elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}