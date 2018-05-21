using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregate that folds matching facts into a collection.
    /// </summary>
    /// <typeparam name="TElement">Type of elements to collect.</typeparam>
    internal class CollectionAggregator<TElement> : IAggregator
    {
        private readonly FactCollection<TElement> _items = new FactCollection<TElement>();
        private bool _created = false;

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            AddFacts(facts);
            if (!_created)
            {
                _created = true;
                return new[] {AggregationResult.Added(_items, _items.Facts)};
            }
            return new[] {AggregationResult.Modified(_items, _items, _items.Facts)};
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            ModifyFacts(facts);
            return new[] {AggregationResult.Modified(_items, _items, _items.Facts)};
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            RemoveFacts(facts);
            return new[] {AggregationResult.Modified(_items, _items, _items.Facts)};
        }

        private void AddFacts(IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact.Value;
                _items.Add(fact, item);
            }
        }

        private void ModifyFacts(IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact.Value;
                _items.Modify(fact, item);
            }
        }

        private void RemoveFacts(IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement) fact.Value;
                _items.Remove(fact, item);
            }
        }
    }
}