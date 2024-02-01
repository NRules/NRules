using System.Collections.Generic;
using NRules.Aggregators.Collections;
using NRules.RuleModel;

namespace NRules.Aggregators;

/// <summary>
/// Aggregate that folds matching facts into a collection.
/// </summary>
/// <typeparam name="TElement">Type of elements to collect.</typeparam>
internal class CollectionAggregator<TElement> : IAggregator
{
    private readonly FactCollection<TElement?> _items = new();
    private bool _created = false;

    public IReadOnlyCollection<AggregationResult> Add(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        AddFacts(facts);
        if (!_created)
        {
            _created = true;
            return new[] {AggregationResult.Added(_items, _items.Facts)};
        }
        return new[] {AggregationResult.Modified(_items, _items, _items.Facts)};
    }

    public IReadOnlyCollection<AggregationResult> Modify(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        ModifyFacts(facts);
        return new[] {AggregationResult.Modified(_items, _items, _items.Facts)};
    }

    public IReadOnlyCollection<AggregationResult> Remove(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        RemoveFacts(facts);
        return new[] {AggregationResult.Modified(_items, _items, _items.Facts)};
    }

    private void AddFacts(IReadOnlyCollection<IFact> facts)
    {
        foreach (var fact in facts)
        {
            var item = (TElement?)fact.Value;
            _items.Add(fact, item);
        }
    }

    private void ModifyFacts(IReadOnlyCollection<IFact> facts)
    {
        foreach (var fact in facts)
        {
            var item = (TElement?)fact.Value;
            _items.Modify(fact, item);
        }
    }

    private void RemoveFacts(IReadOnlyCollection<IFact> facts)
    {
        foreach (var fact in facts)
        {
            _items.Remove(fact);
        }
    }
}