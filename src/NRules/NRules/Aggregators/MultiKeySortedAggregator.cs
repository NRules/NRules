using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Aggregators
{
    /// <summary>
    /// A key selecting expression and direction used for sorting
    /// </summary>
    internal class SortCriteria
    {
        /// <summary>
        /// Details about the sort criteria used in a multi-key sort.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="direction"></param>
        public SortCriteria(IAggregateExpression expression, SortDirection direction)
        {
            KeySelector = expression;
            Direction = direction;
        }

        /// <summary>
        /// The key selection expression used for sorting. 
        /// </summary>
        public IAggregateExpression KeySelector { get; }

        /// <summary>
        /// The direction to sort in.
        /// </summary>
        public SortDirection Direction { get; }
    }

    /// <summary>
    /// Aggregate that adds matching facts into a collection sorted by a given key selector and sort direction.
    /// </summary>
    /// <typeparam name="TSource">Type of elements to collect.</typeparam>
    internal class MultiKeySortedAggregator<TSource> : SortedAggregatorBase<TSource, object[]>
    {
        private readonly SortCriteria[] _sortCriterias;

        public MultiKeySortedAggregator(IEnumerable<SortCriteria> sortCriterias)
            : base(GetComparer(sortCriterias))
        {
            _sortCriterias = sortCriterias.ToArray();
        }

        private static IComparer<object[]> GetComparer(IEnumerable<SortCriteria> sortCriterias)
        {
            var comparers = new List<IComparer<object>>();
            foreach (var sortCriteria in sortCriterias)
            {
                var defaultComparer = (IComparer<object>)Comparer<object>.Default;
                var comparer = sortCriteria.Direction == SortDirection.Ascending ? defaultComparer : new ReverseComparer<object>(defaultComparer);
                comparers.Add(comparer);
            }

            return new MultiKeyComparer(comparers);
        }

        protected override object[] GetKey(AggregationContext context, ITuple tuple, IFact fact)
        {
            return _sortCriterias.Select(x => x.KeySelector.Invoke(context, tuple, fact)).ToArray();
        }
    }

    internal class MultiKeyComparer : IComparer<object[]>
    {
        readonly IComparer<object>[] _comparers;

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