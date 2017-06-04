using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Base interface for fact aggregators.
    /// </summary>
    public interface IAggregator
    {
        /// <summary>
        /// Called by the rules engine when new facts enter corresponding aggregator.
        /// </summary>
        /// <param name="tuple">Tuple containing preceding partial matches.</param>
        /// <param name="facts">New facts to add to the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the added facts.</returns>
        IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts);

        /// <summary>
        /// Called by the rules engine when existing facts are modified in the corresponding aggregator.
        /// </summary>
        /// <param name="tuple">Tuple containing preceding partial matches.</param>
        /// <param name="facts">Existing facts to update in the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the modified facts.</returns>
        IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts);

        /// <summary>
        /// Called by the rules engine when existing facts are removed from the corresponding aggregator.
        /// </summary>
        /// <param name="tuple">Tuple containing preceding partial matches.</param>
        /// <param name="facts">Existing facts to remove from the aggregate.</param>
        /// <returns>Results of the operation on the aggregate, based on the removed facts.</returns>
        IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts);

        /// <summary>
        /// Resulting aggregates.
        /// </summary>
        IEnumerable<object> Aggregates { get; } 
    }
}