using System.Collections.Generic;

namespace NRules.RuleModel.Aggregators
{
    /// <summary>
    /// Aggregate that folds matching facts into a collection.
    /// </summary>
    /// <typeparam name="TElement">Type of elements to collect.</typeparam>
    internal class CollectionAggregator<TElement> : IAggregator
    {
        private FactCollection<TElement> _items; 

        public IEnumerable<AggregationResult> Add(IEnumerable<object> facts)
        {
            if (_items == null)
            {
                _items = new FactCollection<TElement>();
                AddFacts(facts);
                return new[] {AggregationResult.Added(_items)};
            }
            else
            {
                AddFacts(facts);
                return new[] {AggregationResult.Modified(_items)};
            }
        }

        public IEnumerable<AggregationResult> Modify(IEnumerable<object> facts)
        {
            ModifyFacts(facts);
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Remove(IEnumerable<object> facts)
        {
            RemoveFacts(facts);
            return new[] {AggregationResult.Modified(_items)};
        }

        private void AddFacts(IEnumerable<object> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact;
                _items.Add(item);
            }
        }

        private void ModifyFacts(IEnumerable<object> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact;
                _items.Modify(item);
            }
        }

        private void RemoveFacts(IEnumerable<object> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement) fact;
                _items.Remove(item);
            }
        }

        public IEnumerable<object> Aggregates { get { return new[] {_items}; } }
    }
}