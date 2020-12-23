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
        
        private readonly Dictionary<IFact, TKey> _sourceToKey = new Dictionary<IFact, TKey>();
        private readonly Dictionary<IFact, TElement> _sourceToElement = new Dictionary<IFact, TElement>();

        private readonly DefaultKeyMap<TKey, Grouping> _groups = new DefaultKeyMap<TKey, Grouping>();

        public GroupByAggregator(IAggregateExpression keySelector, IAggregateExpression elementSelector)
        {
            _keySelector = keySelector;
            _elementSelector = elementSelector;
        }

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var key = (TKey)_keySelector.Invoke(context, tuple, fact);
                var element = (TElement)_elementSelector.Invoke(context, tuple, fact);
                _sourceToKey[fact] = key;
                _sourceToElement[fact] = element;
                var result = Add(fact, key, element);
                if (!resultLookup.ContainsKey(key))
                {
                    keys.Add(key);
                    resultLookup[key] = result;
                }
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            var removedKeys = new HashSet<TKey>();
            foreach (var fact in facts)
            {
                var key = (TKey)_keySelector.Invoke(context, tuple, fact);
                var element = (TElement)_elementSelector.Invoke(context, tuple, fact);
                var oldKey = _sourceToKey[fact];
                var oldElement = _sourceToElement[fact];
                _sourceToKey[fact] = key;
                _sourceToElement[fact] = element;
        
                if (Equals(key, oldKey))
                {
                    var result = Modify(fact, key, element);
                    if (!resultLookup.ContainsKey(key))
                    {
                        keys.Add(key);
                        resultLookup[key] = result;
                    }
                }
                else
                {
                    var result1 = RemoveElementOnly(fact, oldKey, oldElement);
                    if (!resultLookup.ContainsKey(oldKey))
                    {
                        keys.Add(oldKey);
                    }
                    if (result1.Action == AggregationAction.Removed)
                    {
                        removedKeys.Add(oldKey);
                    }
                    resultLookup[oldKey] = result1;

                    var result2 = Add(fact, key, element);
                    if (!resultLookup.TryGetValue(key, out var previousResult))
                    {
                        keys.Add(key);
                        resultLookup[key] = result2;
                    }
                    else if (previousResult.Action == AggregationAction.Removed)
                    {
                        resultLookup[key] = result2;
                        removedKeys.Remove(key);
                    }
                }
            }

            foreach (var removedKey in removedKeys)
            {
                _groups.Remove(removedKey);
            }

            var results = GetResults(keys, resultLookup);
            return results;
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var oldKey = _sourceToKey[fact];
                var oldElement = _sourceToElement[fact];
                _sourceToKey.Remove(fact);
                _sourceToElement.Remove(fact);
                var result = Remove(fact, oldKey, oldElement);
                if (!resultLookup.ContainsKey(oldKey))
                {
                    keys.Add(oldKey);
                }
                resultLookup[oldKey] = result;
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        private AggregationResult Add(IFact fact, TKey key, TElement element)
        {
            if (!_groups.TryGetValue(key, out var group))
            {
                group = new Grouping(key);
                _groups[key] = group;

                group.Add(fact, element);
                return AggregationResult.Added(group, group.Facts);
            }

            group.Add(fact, element);
            group.Key = key;
            return AggregationResult.Modified(group, group, group.Facts);
        }

        private AggregationResult Modify(IFact fact, TKey key, TElement element)
        {
            var group = _groups[key];
            group.Modify(fact, element);
            group.Key = key;
            return AggregationResult.Modified(group, group, group.Facts);
        }

        private AggregationResult Remove(IFact fact, TKey key, TElement element)
        {
            var group = _groups[key];
            group.Remove(fact, element);
            if (group.Count == 0)
            {
                _groups.Remove(key);
                return AggregationResult.Removed(group);
            }
            return AggregationResult.Modified(group, group, group.Facts);
        }

        private AggregationResult RemoveElementOnly(IFact fact, TKey key, TElement element)
        {
            var group = _groups[key];
            group.Remove(fact, element);
            if (group.Count == 0)
            {
                return AggregationResult.Removed(group);
            }
            return AggregationResult.Modified(group, group, group.Facts);
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

        private class Grouping : FactCollection<TElement>, IGrouping<TKey, TElement>
        {
            public Grouping(TKey key)
            {
                Key = key;
            }

            public TKey Key { get; set; }
        }
    }
}