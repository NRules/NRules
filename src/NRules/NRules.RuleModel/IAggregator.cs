using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base interface for fact aggregators.
    /// </summary>
    public interface IAggregator
    {
        /// <summary>
        /// Called when the new aggregator is initialized.
        /// </summary>
        /// <returns>Results of the operation on the aggregate.</returns>
        IEnumerable<AggregationResult> Initial();

        /// <summary>
        /// Called by the rules engine when a new fact enters corresponding aggregator.
        /// </summary>
        /// <param name="fact">New fact to add to the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the added fact.</returns>
        IEnumerable<AggregationResult> Add(object fact);

        /// <summary>
        /// Called by the rules engine when an existing fact is modified in the corresponding aggregatosr.
        /// </summary>
        /// <param name="fact">Existing fact to update in the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the modified fact.</returns>
        IEnumerable<AggregationResult> Modify(object fact);

        /// <summary>
        /// Called by the rules engine when an existing fact is removed from the corresponding aggregator.
        /// </summary>
        /// <param name="fact">Existing fact to remove from the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the removed fact.</returns>
        IEnumerable<AggregationResult> Remove(object fact);

        /// <summary>
        /// Resulting aggregates.
        /// </summary>
        IEnumerable<object> Aggregates { get; } 
    }
}