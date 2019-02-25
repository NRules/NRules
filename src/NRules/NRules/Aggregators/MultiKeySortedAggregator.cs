using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Aggregators
{
    internal class SortCondition
    {
        public SortCondition(string name, SortDirection direction, IAggregateExpression expression)
        {
            Name = name;
            KeySelector = expression;
            Direction = direction;
        }

        public string Name { get; }
        public IAggregateExpression KeySelector { get; }
        public SortDirection Direction { get; }
    }

    /// <summary>
    /// Aggregate that adds matching facts into a collection sorted by a given key selector and sort direction.
    /// </summary>
    /// <typeparam name="TSource">Type of elements to collect.</typeparam>
    internal class MultiKeySortedAggregator<TSource> : IAggregator
    {
        private readonly SortCondition[] _sortConditions;
        private readonly SortedFactCollection<TSource, object[]> _sortedFactCollection;
        private bool _created = false;

        public MultiKeySortedAggregator(IEnumerable<SortCondition> sortConditions)
        {
            _sortConditions = sortConditions.ToArray();
            var comparer = CreateComparer(_sortConditions);
            _sortedFactCollection = new SortedFactCollection<TSource, object[]>(comparer);
        }

        private static IComparer<object[]> CreateComparer(IEnumerable<SortCondition> sortConditions)
        {
            var comparers = new List<IComparer<object>>();
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
                key[i] = _sortConditions[i].KeySelector.Invoke(context, tuple, fact);
            }
            return key;
        }

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

        private class MultiKeyComparer : IComparer<object[]>
        {
            private readonly IComparer<object>[] _comparers;

            public MultiKeyComparer(IEnumerable<IComparer<object>> comparers)
            {
                _comparers = comparers.ToArray();
            }

            public int Compare(object[] x, object[] y)
            {
                var result = 0;

                for (int i = 0; i < _comparers.Length; i++)
                {
                    result = _comparers[i].Compare(x[i], y[i]);
                    if (result != 0) break;
                }

                return result;
            }
        }
    }
}