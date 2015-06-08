using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate that folds matching facts into a collection.
    /// </summary>
    /// <typeparam name="TElement">Type of elements to collect.</typeparam>
    internal class CollectionAggregator<TElement> : IAggregator
    {
        private readonly FactCollection<TElement> _items = new FactCollection<TElement>(); 

        public IEnumerable<AggregationResult> Initial()
        {
            return new[] {AggregationResult.Added(_items)};
        }

        public IEnumerable<AggregationResult> Add(object fact)
        {
            var item = (TElement) fact;
            _items.Add(item);
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Modify(object fact)
        {
            var item = (TElement)fact;
            _items.Modify(item);
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Remove(object fact)
        {
            var item = (TElement)fact;
            _items.Remove(item);
            return new[] {AggregationResult.Modified(_items)};
        }

        public IEnumerable<object> Aggregates { get { return new[] {_items}; } }
    }
}