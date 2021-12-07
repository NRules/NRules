using System.Collections.Generic;
using NRules.Aggregators.Collections;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator that folds matching facts into a lookup collection, where facts with the same key are grouped together.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements to group.</typeparam>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TElement">Type of elements to group.</typeparam>
    internal class LookupAggregator<TSource, TKey, TElement> : IAggregator
    {
        private readonly IAggregateExpression _keySelector;
        private readonly IAggregateExpression _elementSelector;

        private readonly Dictionary<IFact, TKey> _sourceToKey = new();

        private readonly FactLookup<TKey, TElement> _lookup = new();
        private bool _created = false;

        public LookupAggregator(IAggregateExpression keySelector, IAggregateExpression elementSelector)
        {
            _keySelector = keySelector;
            _elementSelector = elementSelector;
        }

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var key = (TKey)_keySelector.Invoke(context, tuple, fact);
                var element = (TElement)_elementSelector.Invoke(context, tuple, fact);

                _sourceToKey[fact] = key;
                
                AddFact(fact, key, element);
            }

            if (!_created)
            {
                _created = true;
                return new[] {AggregationResult.Added(_lookup, _lookup.Facts)};
            }
            return new[] {AggregationResult.Modified(_lookup, _lookup, _lookup.Facts)};
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var oldKey = _sourceToKey[fact];
                var key = (TKey)_keySelector.Invoke(context, tuple, fact);
                var element = (TElement)_elementSelector.Invoke(context, tuple, fact);

                if (Equals(key, oldKey))
                {
                    ModifyFact(fact, key, element);
                }
                else
                {
                    RemoveFact(fact, oldKey);
                    AddFact(fact, key, element);
                }
            }

            return new[] {AggregationResult.Modified(_lookup, _lookup, _lookup.Facts)};
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var key = _sourceToKey[fact];
                RemoveFact(fact, key);
            }

            return new[] {AggregationResult.Modified(_lookup, _lookup, _lookup.Facts)};
        }

        private void AddFact(IFact fact, TKey key, TElement element)
        {
            if (!_lookup.TryGetValue(key, out var grouping))
            {
                grouping = new FactGrouping<TKey, TElement>(key);
                _lookup.Add(key, grouping);
            }
            else
            {
                grouping.Key = key;
            }

            grouping.Add(fact, element);
        }

        private void ModifyFact(IFact fact, TKey key, TElement element)
        {
            var grouping = _lookup[key];
            grouping.Key = key;
            grouping.Modify(fact, element);
        }

        private void RemoveFact(IFact fact, TKey key)
        {
            var grouping = _lookup[key];
            grouping.Remove(fact);
            if (grouping.Count == 0)
            {
                _lookup.Remove(key);
            }
        }
    }
}