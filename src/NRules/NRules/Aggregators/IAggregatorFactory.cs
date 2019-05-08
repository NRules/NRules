using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Base interface for aggregator factories.
    /// Aggregator factory constructs new instances of <see cref="IAggregator"/> of a given type, so that they
    /// can accumulate aggregation results.
    /// An <c>IAggregatorFactory</c> type must either be registered in <see cref="RuleCompiler.AggregatorRegistry"/>,
    /// or provided in the canonical rule model via <see cref="AggregateElement.CustomFactoryType"/>. If both are
    /// provided, the aggregator factory at the <see cref="AggregateElement"/> level takes precedence.
    /// </summary>
    public interface IAggregatorFactory
    {
        /// <summary>
        /// Called by the rules engine to compile the aggregator factory before it is used for the first time.
        /// </summary>
        /// <param name="element">Corresponding aggregate element from the rule definition.</param>
        /// <param name="compiledExpressions">Aggregate expressions compiled to an executable form.</param>
        void Compile(AggregateElement element, IEnumerable<IAggregateExpression> compiledExpressions);

        /// <summary>
        /// Creates a new aggregator instance.
        /// This method is called by the engine for each new combination of preceding partial matches, 
        /// so that a new instance of the aggregator is created to accummulate the results.
        /// </summary>
        /// <returns>Aggregator instance.</returns>
        IAggregator Create();
    }
}