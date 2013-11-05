namespace NRules.RuleModel
{
    /// <summary>
    /// Result of the aggregation, based on a fact change.
    /// </summary>
    public enum AggregationResults
    {
        /// <summary>
        /// No changes at the aggregate level.
        /// </summary>
        None = 0,

        /// <summary>
        /// New aggregate created.
        /// </summary>
        Added = 1,

        /// <summary>
        /// Existing aggregate modified.
        /// </summary>
        Modified = 2,

        /// <summary>
        /// Existing aggregate removed.
        /// </summary>
        Removed = 3,
    }

    /// <summary>
    /// Base interface for aggregate types.
    /// </summary>
    public interface IAggregate
    {
        /// <summary>
        /// Add is called by the rules engine when a new fact enters an aggregation node.
        /// </summary>
        /// <param name="fact">New fact to add to the aggregate.</param>
        /// <returns>Result on the aggregate, based on the added fact.</returns>
        AggregationResults Add(object fact);

        /// <summary>
        /// Modify is called by the rules engine when an existing updated fact enters an aggregation node.
        /// </summary>
        /// <param name="fact">Existing fact to update in the aggregate.</param>
        /// <returns>Result on the aggregate, based on the modified fact.</returns>
        AggregationResults Modify(object fact);

        /// <summary>
        /// Remove is called by the rules engine when an existing retracted fact enters an aggregation node.
        /// </summary>
        /// <param name="fact">Existing fact to remove from the aggregate.</param>
        /// <returns>Result on the aggregate, based on the removed fact.</returns>
        AggregationResults Remove(object fact);

        /// <summary>
        /// Result of the aggregation.
        /// </summary>
        object Result { get; }
    }
}