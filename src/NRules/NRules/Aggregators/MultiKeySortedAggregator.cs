using System.Collections.Generic;
using NRules.Aggregators.Collections;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Aggregators;

/// <summary>
/// Aggregate that adds matching facts into a collection sorted by a given key selector and sort direction.
/// </summary>
/// <typeparam name="TSource">Type of elements to collect.</typeparam>
internal class MultiKeySortedAggregator<TSource> : IAggregator
{
    private readonly SortCondition[] _sortConditions;
    private readonly SortedFactCollection<TSource, object[]> _sortedFactCollection;
    private bool _created = false;

    public MultiKeySortedAggregator(SortCondition[] sortConditions)
    {
        _sortConditions = sortConditions;
        var comparer = CreateComparer(_sortConditions);
        _sortedFactCollection = new SortedFactCollection<TSource, object[]>(comparer);
    }

    private static IComparer<object[]> CreateComparer(IReadOnlyCollection<SortCondition> sortConditions)
    {
        var comparers = new List<IComparer<object>>(sortConditions.Count);
        foreach (var sortCondition in sortConditions)
        {
            var defaultComparer = (IComparer<object>)Comparer<object>.Default;
            var comparer = sortCondition.Direction == SortDirection.Ascending ? defaultComparer : new ReverseComparer<object>(defaultComparer);
            comparers.Add(comparer);
        }

        return new MultiKeyComparer(comparers);
    }

    private object[] GetKey(AggregationContext context, ITuple tuple, IFact fact)
    {
        var key = new object[_sortConditions.Length];
        for (int i = 0; i < _sortConditions.Length; i++)
        {
            key[i] = _sortConditions[i].Expression.Invoke(context, tuple, fact);
        }
        return key;
    }

    public IReadOnlyCollection<AggregationResult> Add(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        AddFacts(context, tuple, facts);
        if (!_created)
        {
            _created = true;
            return new[] { AggregationResult.Added(_sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
        }
        return new[] { AggregationResult.Modified(_sortedFactCollection, _sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
    }

    public IReadOnlyCollection<AggregationResult> Modify(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        ModifyFacts(context, tuple, facts);
        return new[] { AggregationResult.Modified(_sortedFactCollection, _sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
    }

    public IReadOnlyCollection<AggregationResult> Remove(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        RemoveFacts(context, tuple, facts);
        return new[] { AggregationResult.Modified(_sortedFactCollection, _sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
    }

    private void AddFacts(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        foreach (var fact in facts)
        {
            var key = GetKey(context, tuple, fact);
            _sortedFactCollection.AddFact(key, fact);
        }
    }

    private void ModifyFacts(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        foreach (var fact in facts)
        {
            _sortedFactCollection.RemoveFact(fact);

            var key = GetKey(context, tuple, fact);
            _sortedFactCollection.AddFact(key, fact);
        }
    }

    private void RemoveFacts(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        foreach (var fact in facts)
        {
            _sortedFactCollection.RemoveFact(fact);
        }
    }

    private class MultiKeyComparer : IComparer<object[]>
    {
        private readonly IReadOnlyList<IComparer<object>> _comparers;

        public MultiKeyComparer(IReadOnlyList<IComparer<object>> comparers)
        {
            _comparers = comparers;
        }

        public int Compare(object[] x, object[] y)
        {
            int result = 0;

            for (int i = 0; i < _comparers.Count; i++)
            {
                result = _comparers[i].Compare(x[i], y[i]);
                if (result != 0) break;
            }

            return result;
        }
    }
}