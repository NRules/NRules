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
        private readonly object[] _container; 
        private bool _created = false;

        public CollectionAggregator()
        {
            _container = new object[] {_items};
        } 

        public IEnumerable<AggregationResult> Add(IEnumerable<object> facts)
        {
            AddFacts(facts);
            if (!_created)
            {
                _created = true;
                return new[] {AggregationResult.Added(_items)};
            }
            return new[] {AggregationResult.Modified(_items)};
        }

        public IEnumerable<AggregationResult> Modify(IEnumerable<object> facts)
        {
            ModifyFacts(facts);
            return new[] {AggregationResult.Modified(_items)};
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

        public IEnumerable<object> Aggregates { get { return _container; } }
    }
}