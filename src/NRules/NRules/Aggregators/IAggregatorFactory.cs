using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Base interface for aggregator factories.
    /// </summary>
    public interface IAggregatorFactory
    {
        /// <summary>
        /// Called by the rules engine to compile the aggregator factory before it is used for the first time.
        /// </summary>
        /// <param name="element">Corresponding aggregate element from the rule definition.</param>
        /// <param name="compiledExpressions">Aggregate expressions compiled to an executable form.</param>
        void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions);

        /// <summary>
        /// Creates a new aggregator instance.
        /// This method is called by the engine for each new combination of preceding partial matches, 
        /// so that a new instance of the aggregator is created to accummulate the results.
        /// </summary>
        /// <returns>Aggregator instance.</returns>
        IAggregator Create();
    }
}