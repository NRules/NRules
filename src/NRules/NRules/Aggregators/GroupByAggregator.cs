using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator that groups matching facts into collections of elements with the same key.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TElement">Type of elements to group.</typeparam>
    internal class GroupByAggregator<TSource, TKey, TElement> : IAggregator
    {
        private readonly IAggregateExpression _keySelector;
        private readonly IAggregateExpression _elementSelector;
        
        private readonly Dictionary<object, TKey> _sourceToKey = new Dictionary<object, TKey>();
        private readonly Dictionary<object, TElement> _sourceToElement = new Dictionary<object, TElement>();

        private readonly DefaultKeyMap<TKey, Grouping> _groups = new DefaultKeyMap<TKey, Grouping>();

        public GroupByAggregator(IAggregateExpression keySelector, IAggregateExpression elementSelector)
        {
            _keySelector = keySelector;
            _elementSelector = elementSelector;
        }

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var key = (TKey)_keySelector.Invoke(tuple, fact);
                var element = (TElement)_elementSelector.Invoke(tuple, fact);
                _sourceToKey[source] = key;
                _sourceToElement[source] = element;
                var result = Add(key, element);
                if (!resultLookup.ContainsKey(key))
                {
                    keys.Add(key);
                    resultLookup[key] = result;
                }
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var key = (TKey)_keySelector.Invoke(tuple, fact);
                var element = (TElement)_elementSelector.Invoke(tuple, fact);
                var oldKey = _sourceToKey[source];
                var oldElement = _sourceToElement[source];
                _sourceToKey[source] = key;
                _sourceToElement[source] = element;
        
                if (Equals(key, oldKey))
                {
                    var result = Modify(key, oldElement, element);
                    if (!resultLookup.ContainsKey(key))
                    {
                        keys.Add(key);
                        resultLookup[key] = result;
                    }
                }
                else
                {
                    var result1 = Remove(oldKey, oldElement);
                    if (!resultLookup.ContainsKey(oldKey))
                    {
                        keys.Add(oldKey);
                    }
                    resultLookup[oldKey] = result1;

                    var result2 = Add(key, element);
                    AggregationResult previousResult;
                    if (!resultLookup.TryGetValue(key, out previousResult))
                    {
                        keys.Add(key);
                        resultLookup[key] = result2;
                    }
                    else if (previousResult.Action == AggregationAction.Removed ||
                             result2.Action == AggregationAction.Added)
                    {
                        resultLookup[key] = AggregationResult.Modified(previousResult.Aggregate);
                    }
                }
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var oldKey = _sourceToKey[source];
                var oldElement = _sourceToElement[source];
                _sourceToKey.Remove(source);
                _sourceToElement.Remove(source);
                var result = Remove(oldKey, oldElement);
                if (!resultLookup.ContainsKey(oldKey))
                {
                    keys.Add(oldKey);
                }
                resultLookup[oldKey] = result;
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        private AggregationResult Add(TKey key, TElement element)
        {
            Grouping group;
            if (!_groups.TryGetValue(key, out group))
            {
                group = new Grouping(key);
                _groups[key] = group;

                group.Add(element);
                return AggregationResult.Added(group);
            }

            group.Add(element);
            return AggregationResult.Modified(group);
        }

        private AggregationResult Modify(TKey key, TElement oldElement, TElement element)
        {
            var group = _groups[key];
            if (Equals(oldElement, element))
            {
                group.Modify(element);
            }
            else
            {
                group.Remove(oldElement);
                group.Add(element);
            }
            return AggregationResult.Modified(group);
        }

        private AggregationResult Remove(TKey key, TElement element)
        {
            var group = _groups[key];
            group.Remove(element);
            if (group.Count == 0)
            {
                _groups.Remove(key);
                return AggregationResult.Removed(group);
            }
            return AggregationResult.Modified(group);
        }

        private static IEnumerable<AggregationResult> GetResults(IEnumerable<TKey> keys, DefaultKeyMap<TKey, AggregationResult> lookup)
        {
            var results = new List<AggregationResult>();
            foreach (var key in keys)
            {
                var result = lookup[key];
                results.Add(result);
            }
            return results;
        }

        public IEnumerable<object> Aggregates => _groups.Values;

        private class Grouping : FactCollection<TElement>, IGrouping<TKey, TElement>
        {
            public Grouping(TKey key)
            {
                Key = key;
            }

            public TKey Key { get; }
        }
    }
}