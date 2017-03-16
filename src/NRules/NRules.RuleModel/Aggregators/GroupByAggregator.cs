using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregator that groups matching facts into collections of elements with the same key.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TElement">Type of elements to group.</typeparam>
    internal class GroupByAggregator<TSource, TKey, TElement> : IAggregator
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        
        private readonly Dictionary<object, TKey> _sourceToKey = new Dictionary<object, TKey>();
        private readonly Dictionary<object, TElement> _sourceToElement = new Dictionary<object, TElement>();

        private readonly Dictionary<TKey, Grouping> _groups = new Dictionary<TKey, Grouping>();

        private readonly TKey _defaultKey = default(TKey);
        private Grouping _defaultGroup;

        public GroupByAggregator(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            _keySelector = keySelector;
            _elementSelector = elementSelector;
        }

        public IEnumerable<AggregationResult> Initial()
        {
            return AggregationResult.Empty;
        }

        public IEnumerable<AggregationResult> Add(IEnumerable<object> facts)
        {
            var keyElements = new List<KeyValuePair<TKey, TElement>>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var key = _keySelector(source);
                var element = _elementSelector(source);
                _sourceToKey[fact] = key;
                _sourceToElement[fact] = element;
                var keyElement = new KeyValuePair<TKey, TElement>(key, element);
                keyElements.Add(keyElement);
            }
            return Add(keyElements);
        }

        public IEnumerable<AggregationResult> Modify(IEnumerable<object> facts)
        {
            var keyElementsToUpdate = new List<KeyValuePair<TKey, TElement>>();
            var keyElementsToAdd = new List<KeyValuePair<TKey, TElement>>();
            var keyElementsToRemove = new List<KeyValuePair<TKey, TElement>>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact;
                var key = _keySelector(source);
                var element = _elementSelector(source);
                var oldKey = _sourceToKey[fact];
                var oldElement = _sourceToElement[fact];
                _sourceToKey[fact] = key;
                _sourceToElement[fact] = element;

                if (Equals(key, oldKey))
                {
                    var keyElementToModify = new KeyValuePair<TKey, TElement>(key, element);
                    keyElementsToUpdate.Add(keyElementToModify);
                    continue;
                }

                var keyElementToRemove = new KeyValuePair<TKey, TElement>(oldKey, oldElement);
                keyElementsToRemove.Add(keyElementToRemove);

                var keyElementToInsert = new KeyValuePair<TKey, TElement>(key, element);
                keyElementsToAdd.Add(keyElementToInsert);
            }
            var results = new List<AggregationResult>();
            var additionResults = Add(keyElementsToAdd);
            var modifiedResults = Modify(keyElementsToUpdate);
            var removeResults = Remove(keyElementsToRemove);
            results.AddRange(modifiedResults);
            results.AddRange(additionResults);
            results.AddRange(removeResults);
            return results;
        }

        public IEnumerable<AggregationResult> Remove(IEnumerable<object> facts)
        {
            var keyElementsToRemove = new List<KeyValuePair<TKey, TElement>>();
            foreach (var fact in facts)
            {
                var oldKey = _sourceToKey[fact];
                var oldElement = _sourceToElement[fact];
                _sourceToKey.Remove(fact);
                _sourceToElement.Remove(fact);
                keyElementsToRemove.Add(new KeyValuePair<TKey, TElement>(oldKey, oldElement));
            }
            return Remove(keyElementsToRemove);
        }

        private IEnumerable<AggregationResult> Add(List<KeyValuePair<TKey, TElement>> keyElements)
        {
            var groupsToAdd = new HashSet<Grouping>();
            var groupsToModify = new HashSet<Grouping>();
            foreach (var keyElement in keyElements)
            {
                var key = keyElement.Key;
                var element = keyElement.Value;
                if (Equals(key, _defaultKey))
                {
                    if (_defaultGroup == null)
                    {
                        _defaultGroup = new Grouping(key);
                        _defaultGroup.Add(element);
                        groupsToAdd.Add(_defaultGroup);
                        continue;
                    }
                    _defaultGroup.Add(element);
                    groupsToModify.Add(_defaultGroup);
                    continue;
                }

                Grouping group;
                if (!_groups.TryGetValue(key, out group))
                {
                    group = new Grouping(key);
                    _groups[key] = group;

                    group.Add(element);
                    groupsToAdd.Add(group);
                    continue;
                }

                group.Add(element);
                groupsToModify.Add(group);
                continue;
            }
            var actualGroupsToModify = groupsToModify.Except(groupsToAdd);
            var aggregationResults = groupsToAdd.Select(AggregationResult.Added).ToList();
            aggregationResults.AddRange(actualGroupsToModify.Select(AggregationResult.Modified));
            return aggregationResults;
        }

        private IEnumerable<AggregationResult> Modify(List<KeyValuePair<TKey, TElement>> keyElements)
        {
            var groupsToModify = new HashSet<Grouping>();
            foreach (var keyElement in keyElements)
            {
                var key = keyElement.Key;
                var element = keyElement.Value;
                if (Equals(key, _defaultKey))
                {
                    _defaultGroup.Modify(element);
                    groupsToModify.Add(_defaultGroup);
                    continue;
                }

                var group = _groups[key];
                group.Modify(element);
                groupsToModify.Add(group);
            }
            return groupsToModify.Select(AggregationResult.Modified);
        }

        private IEnumerable<AggregationResult> Remove(List<KeyValuePair<TKey, TElement>> keyElements)
        {
            var groupsToRemove = new HashSet<Grouping>();
            var groupsToModify = new HashSet<Grouping>();
            foreach (var keyElement in keyElements)
            {
                var key = keyElement.Key;
                var element = keyElement.Value;
                if (Equals(key, _defaultKey))
                {
                    _defaultGroup.Remove(element);
                    if (_defaultGroup.Count == 0)
                    {
                        var removedGroup = _defaultGroup;
                        _defaultGroup = null;
                        groupsToRemove.Add(removedGroup);
                        continue;
                    }
                    groupsToModify.Add(_defaultGroup);
                    continue;
                }

                var group = _groups[key];
                group.Remove(element);
                if (group.Count == 0)
                {
                    _groups.Remove(key);
                    groupsToRemove.Add(group);
                    continue;
                }
                groupsToModify.Add(group);
                continue;
            }
            var actualGroupsToModify = groupsToModify.Except(groupsToRemove);
            var aggregationResults = groupsToRemove.Select(AggregationResult.Removed).ToList();
            aggregationResults.AddRange(actualGroupsToModify.Select(AggregationResult.Modified));
            return aggregationResults;
        }

        public IEnumerable<object> Aggregates
        {
            get
            {
                var aggregates = _defaultGroup == null
                    ? _groups.Values
                    : new[] {_defaultGroup}.Concat(_groups.Values);
                return aggregates;
            }
        }

        private class Grouping : FactCollection<TElement>, IGrouping<TKey, TElement>
        {
            private readonly TKey _key;

            public Grouping(TKey key)
            {
                _key = key;
            }

            public TKey Key { get { return _key; } }
        }
    }
}