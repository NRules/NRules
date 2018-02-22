using System.Collections.Generic;
using System.Diagnostics;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Action that aggregation performed on the aggregate, based on added/modified/removed facts.
    /// </summary>
    public enum AggregationAction
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
    /// Result of the aggregation.
    /// </summary>
    [DebuggerDisplay("{Action}")]
    public class AggregationResult
    {
        public static readonly AggregationResult[] Empty = new AggregationResult[0];

        private AggregationResult(AggregationAction action, object aggregate, object previous, IEnumerable<IFact> source)
        {
            Action = action;
            Aggregate = aggregate;
            Previous = previous;
            Source = source;
        }

        /// <summary>
        /// Constructs an aggregation result that indicates no changes at the aggregate level.
        /// </summary>
        /// <param name="result">Aggregate.</param>
        /// <param name="source">Aggregate source facts.</param>
        /// <returns>Aggregation result.</returns>
        public static AggregationResult None(object result, IEnumerable<IFact> source)
        {
            return new AggregationResult(AggregationAction.None, result, null, source);
        }

        /// <summary>
        /// Constructs an aggregation result that indicates a new aggregate.
        /// </summary>
        /// <param name="result">Aggregate.</param>
        /// <param name="source">Aggregate source facts.</param>
        /// <returns>Aggregation result.</returns>
        public static AggregationResult Added(object result, IEnumerable<IFact> source)
        {
            return new AggregationResult(AggregationAction.Added, result, null, source);
        }

        /// <summary>
        /// Constructs an aggregation result that indicates a modification at the aggregate level.
        /// </summary>
        /// <param name="result">Aggregate.</param>
        /// <param name="previous">Previous aggregate.</param>
        /// <param name="source">Aggregate source facts.</param>
        /// <returns>Aggregation result.</returns>
        public static AggregationResult Modified(object result, object previous, IEnumerable<IFact> source)
        {
            return new AggregationResult(AggregationAction.Modified, result, previous, source);
        }

        /// <summary>
        /// Constructs an aggregation result that indicates an aggregate was removed.
        /// </summary>
        /// <param name="result">Aggregate.</param>
        /// <returns>Aggregation result.</returns>
        public static AggregationResult Removed(object result)
        {
            return new AggregationResult(AggregationAction.Removed, result, result, null);
        }

        /// <summary>
        /// Action that aggregation performed on the aggregate.
        /// </summary>
        public AggregationAction Action { get; }

        /// <summary>
        /// Resulting aggregate.
        /// </summary>
        public object Aggregate { get; }
        
        /// <summary>
        /// Previous aggregate.
        /// </summary>
        public object Previous { get; }

        /// <summary>
        /// Facts that produced this aggregation result.
        /// </summary>
        public IEnumerable<IFact> Source { get; }
    }
}