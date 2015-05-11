using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate that folds matching facts into a collection.
    /// </summary>
    /// <typeparam name="TFact">Type of facts to collect.</typeparam>
    internal class CollectionAggregator<TFact> : IAggregator
    {
        private readonly List<TFact> _items = new List<TFact>();

        public IEnumerable<AggregationResult> Initial()
        {
            return new[] {AggregationResult.Added(_items)};
        }

        public IEnumerable<AggregationResult> Add(object fact)
        {
            _items.Add((TFact) fact);
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            return AggregationResult.Empty;
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            _items.Remove((TFact) fact);
            return new[] {AggregationResult.Modified(_items)};
        }

        public IEnumerable<object> Aggregates { get { return new[] {_items}; } }
    }
}