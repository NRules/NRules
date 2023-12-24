using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators;

/// <summary>
/// Base interface for fact aggregators.
/// An aggregator is a stateful element of the rules engine, that receives matching facts of a particular kind,
/// and can combine them into a synthetic fact, that is then used by the downstream logic in the rule.
/// Aggregator also receives updates and removals for the matching facts, so that it can keep the corresponding
/// aggregate facts in sync.
/// An aggregator must be supplemented by a corresponding implementation of <see cref="IAggregatorFactory"/> that
/// knows how to create new instances of the aggregator.
/// </summary>
public interface IAggregator
{
    /// <summary>
    /// Called by the rules engine when new facts enter corresponding aggregator.
    /// </summary>
    /// <param name="context">Aggregation context.</param>
    /// <param name="tuple">Tuple containing preceding partial matches.</param>
    /// <param name="facts">New facts to add to the aggregate.</param>
    /// <returns>Results of the operation on the aggregate, based on the added facts.</returns>
    IReadOnlyCollection<AggregationResult> Add(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts);

    /// <summary>
    /// Called by the rules engine when existing facts are modified in the corresponding aggregator.
    /// </summary>
    /// <param name="context">Aggregation context.</param>
    /// <param name="tuple">Tuple containing preceding partial matches.</param>
    /// <param name="facts">Existing facts to update in the aggregate.</param>
    /// <returns>Results of the operation on the aggregate, based on the modified facts.</returns>
    IReadOnlyCollection<AggregationResult> Modify(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts);

    /// <summary>
    /// Called by the rules engine when existing facts are removed from the corresponding aggregator.
    /// </summary>
    /// <param name="context">Aggregation context.</param>
    /// <param name="tuple">Tuple containing preceding partial matches.</param>
    /// <param name="facts">Existing facts to remove from the aggregate.</param>
    /// <returns>Results of the operation on the aggregate, based on the removed facts.</returns>
    IReadOnlyCollection<AggregationResult> Remove(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts);
}