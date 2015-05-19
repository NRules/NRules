using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate that groups matching facts into collections of elements with the same key.
    /// </summary>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TValue">Type of grouping value.</typeparam>
    /// <typeparam name="TFact">Type of facts to group.</typeparam>
    internal class GroupByAggregator<TKey, TValue, TFact> : IAggregator
    {
        private readonly Func<TFact, TKey> _keySelector;
        private readonly Func<TFact, TValue> _valueSelector;
        private readonly Dictionary<TKey, Grouping> _groups = new Dictionary<TKey, Grouping>();
        private readonly Dictionary<object, TKey> _factToKey = new Dictionary<object, TKey>(); 
        private readonly Dictionary<object, TValue> _factToValue = new Dictionary<object, TValue>(); 

        public GroupByAggregator(Func<TFact, TKey> keySelector, Func<TFact, TValue> valueSelector)
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
            var key = _keySelector((TFact) fact);
            var value = _valueSelector((TFact) fact);
            _factToKey[fact] = key;
            _factToValue[fact] = value;
            return Add(key, value);
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            var key = _keySelector((TFact)fact);
            var value = _valueSelector((TFact)fact);
            var oldKey = _factToKey[fact];
            var oldValue = _factToValue[fact];
            _factToKey[fact] = key;
            _factToValue[fact] = value;
            if (Equals(key, oldKey)) return AggregationResult.Empty;

            var result1 = Remove(oldKey, oldValue);
            var result2 = Add(key, value);
            return result1.Union(result2);
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            var oldKey = _factToKey[fact];
            var oldValue = _factToValue[fact];
            _factToKey.Remove(fact);
            _factToValue.Remove(fact);
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

        public IEnumerable<object> Aggregates { get { return new[] { _groups.Values }; } }

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