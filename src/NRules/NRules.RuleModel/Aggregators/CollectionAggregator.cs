using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate that folds matching facts into a collection.
    /// </summary>
    /// <typeparam name="TElement">Type of facts to collect.</typeparam>
    internal class CollectionAggregator<TElement> : IAggregator
    {
        private readonly List<TElement> _items = new List<TElement>();

        public IEnumerable<AggregationResult> Initial()
        {
            return new[] {AggregationResult.Added(_items)};
        }

        public IEnumerable<AggregationResult> Add(object fact)
        {
            _items.Add((TElement) fact);
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            return AggregationResult.Empty;
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            _items.Remove((TElement) fact);
            return new[] {AggregationResult.Modified(_items)};
        }

        public IEnumerable<object> Aggregates { get { return new[] {_items}; } }
    }
}