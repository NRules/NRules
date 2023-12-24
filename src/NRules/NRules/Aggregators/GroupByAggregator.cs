using System.Collections.Generic;
using NRules.Aggregators.Collections;
using NRules.RuleModel;

namespace NRules.Aggregators;

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
    
    private readonly Dictionary<IFact, TKey> _sourceToKey = new();

    private readonly DefaultKeyMap<TKey, FactGrouping<TKey, TElement>> _groups = new();

    public GroupByAggregator(IAggregateExpression keySelector, IAggregateExpression elementSelector)
    {
        _keySelector = keySelector;
        _elementSelector = elementSelector;
    }

    public IReadOnlyCollection<AggregationResult> Add(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        var keys = new List<TKey>();
        var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
        foreach (var fact in facts)
        {
            var key = (TKey)_keySelector.Invoke(context, tuple, fact);
            var element = (TElement)_elementSelector.Invoke(context, tuple, fact);
            _sourceToKey[fact] = key;
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

    public IReadOnlyCollection<AggregationResult> Modify(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        var keys = new List<TKey>();
        var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
        var removedKeys = new HashSet<TKey>();
        foreach (var fact in facts)
        {
            var key = (TKey)_keySelector.Invoke(context, tuple, fact);
            var element = (TElement)_elementSelector.Invoke(context, tuple, fact);
            var oldKey = _sourceToKey[fact];
            _sourceToKey[fact] = key;
    
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
                var result1 = RemoveElementOnly(fact, oldKey);
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

    public IReadOnlyCollection<AggregationResult> Remove(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        var keys = new List<TKey>();
        var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
        foreach (var fact in facts)
        {
            var oldKey = _sourceToKey[fact];
            _sourceToKey.Remove(fact);
            var result = Remove(fact, oldKey);
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
            group = new FactGrouping<TKey, TElement>(key);
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

    private AggregationResult Remove(IFact fact, TKey key)
    {
        var group = _groups[key];
        group.Remove(fact);
        if (group.Count == 0)
        {
            _groups.Remove(key);
            return AggregationResult.Removed(group);
        }
        return AggregationResult.Modified(group, group, group.Facts);
    }

    private AggregationResult RemoveElementOnly(IFact fact, TKey key)
    {
        var group = _groups[key];
        group.Remove(fact);
        if (group.Count == 0)
        {
            return AggregationResult.Removed(group);
        }
        return AggregationResult.Modified(group, group, group.Facts);
    }

    private static IReadOnlyCollection<AggregationResult> GetResults(IReadOnlyCollection<TKey> keys, DefaultKeyMap<TKey, AggregationResult> lookup)
    {
        var results = new List<AggregationResult>(keys.Count);
        foreach (var key in keys)
        {
            var result = lookup[key];
            results.Add(result);
        }
        return results;
    }
}