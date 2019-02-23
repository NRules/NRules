using System.Collections.Generic;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregate that adds matching facts into a collection sorted by a given key selector and sort direction.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TKey">Type of key used in the key selector for sorting.</typeparam>
    internal class SortedAggregator<TSource, TKey> : SortedAggregatorBase<TSource, TKey>
    {
        private readonly IAggregateExpression _keySelector;

        public SortedAggregator(IAggregateExpression keySelector, SortDirection sortDirection)
            : base(GetComparer(sortDirection))
        {
            _keySelector = keySelector;
        }

        private static IComparer<TKey> GetComparer(SortDirection sortDirection)
        {
            var defaultComparer = (IComparer<TKey>)Comparer<TKey>.Default;
            var comparer = sortDirection == SortDirection.Ascending ? defaultComparer : new ReverseComparer<TKey>(defaultComparer);
            return comparer;
        }

        protected override TKey GetKey(AggregationContext context, ITuple tuple, IFact fact)
        {
            return (TKey)_keySelector.Invoke(context, tuple, fact);
        }
    }

    internal abstract class SortedAggregatorBase<TSource, TKey> : IAggregator
    {
        private readonly SortedFactCollection<TSource, TKey> _sortedFactCollection;
        private bool _created = false;

        protected SortedAggregatorBase(IComparer<TKey> comparer)
        {
            _sortedFactCollection = new SortedFactCollection<TSource, TKey>(comparer);
        }

        protected abstract TKey GetKey(AggregationContext context, ITuple tuple, IFact fact);

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            AddFacts(context, tuple, facts);
            if (!_created)
            {
                _created = true;
                return new[] { AggregationResult.Added(_sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
            }
            return new[] { AggregationResult.Modified(_sortedFactCollection, _sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            ModifyFacts(context, tuple, facts);
            return new[] { AggregationResult.Modified(_sortedFactCollection, _sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            RemoveFacts(context, tuple, facts);
            return new[] { AggregationResult.Modified(_sortedFactCollection, _sortedFactCollection, _sortedFactCollection.GetFactEnumerable()) };
        }

        private void AddFacts(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var key = GetKey(context, tuple, fact);
                _sortedFactCollection.AddFact(key, fact);
            }
        }

        private void ModifyFacts(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                _sortedFactCollection.RemoveFact(fact);

                var key = GetKey(context, tuple, fact);
                _sortedFactCollection.AddFact(key, fact);
            }
        }

        private void RemoveFacts(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                _sortedFactCollection.RemoveFact(fact);
            }
        }
    }
}