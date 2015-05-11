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
    /// <typeparam name="TFact">Type of facts to group.</typeparam>
    internal class GroupByAggregator<TKey, TFact> : IAggregator
    {
        private readonly Func<TFact, TKey> _keySelector;
        private readonly Dictionary<TKey, Grouping> _groups = new Dictionary<TKey, Grouping>();
        private readonly Dictionary<object, TKey> _factToKey = new Dictionary<object, TKey>(); 

        public GroupByAggregator(Func<TFact, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public IEnumerable<AggregationResult> Initial()
        {
            return AggregationResult.Empty;
        }

        public IEnumerable<AggregationResult> Add(object fact)
        {
            var key = _keySelector((TFact) fact);
            _factToKey[fact] = key;
            return Add(key, fact);
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            var key = _keySelector((TFact)fact);
            var oldKey = _factToKey[fact];
            _factToKey[fact] = key;
            if (Equals(key, oldKey)) return AggregationResult.Empty;

            var result1 = Remove(oldKey, fact);
            var result2 = Add(key, fact);
            return result1.Union(result2);
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            var oldKey = _factToKey[fact];
            _factToKey.Remove(fact);
            return Remove(oldKey, fact);
        }

        private IEnumerable<AggregationResult> Add(TKey key, object fact)
        {
            Grouping group;
            if (!_groups.TryGetValue(key, out group))
            {
                group = new Grouping(key);
                _groups[key] = group;

                group.Add((TFact)fact);
                return new[] { AggregationResult.Added(group) };
            }

            group.Add((TFact)fact);
            return new[] { AggregationResult.Modified(group) };
        }
        
        private IEnumerable<AggregationResult> Remove(TKey key, object fact)
        {
            var group = _groups[key];
            group.Remove((TFact) fact);
            if (group.Count == 0)
            {
                _groups.Remove(key);
                return new[] { AggregationResult.Removed(group) };
            }

            return new[] { AggregationResult.Modified(group) };
        }

        public IEnumerable<object> Aggregates { get { return new[] { _groups.Values }; } }

        private class Grouping : IGrouping<TKey, TFact>
        {
            private readonly TKey _key;
            private readonly List<TFact> _elements = new List<TFact>();

            public Grouping(TKey key)
            {
                _key = key;
            }

            public TKey Key { get { return _key; } }
            public int Count { get { return _elements.Count; } }

            public void Add(TFact fact)
            {
                _elements.Add(fact);
            }

            public void Remove(TFact fact)
            {
                _elements.Remove(fact);
            }

            public IEnumerator<TFact> GetEnumerator()
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