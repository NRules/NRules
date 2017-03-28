using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base interface for fact aggregators.
    /// </summary>
    public interface IAggregator
    {
        /// <summary>
        /// Called by the rules engine when new facts enter corresponding aggregator.
        /// </summary>
        /// <param name="facts">New facts to add to the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the added facts.</returns>
        IEnumerable<AggregationResult> Add(IEnumerable<object> facts);

        /// <summary>
        /// Called by the rules engine when existing facts are modified in the corresponding aggregator.
        /// </summary>
        /// <param name="facts">Existing facts to update in the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the modified facts.</returns>
        IEnumerable<AggregationResult> Modify(IEnumerable<object> facts);

        /// <summary>
        /// Called by the rules engine when existing facts are removed from the corresponding aggregator.
        /// </summary>
        /// <param name="facts">Existing facts to remove from the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the removed facts.</returns>
        IEnumerable<AggregationResult> Remove(IEnumerable<object> facts);

        /// <summary>
        /// Resulting aggregates.
        /// </summary>
        IEnumerable<object> Aggregates { get; } 
    }
}