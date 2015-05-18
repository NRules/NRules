using System;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder to continue defining a pattern.
    /// </summary>
    /// <typeparam name="TFact">Type of fact that the pattern is for.</typeparam>
    public interface IContinuationConditionExpression<TFact> : IContinuationExpression<TFact>
    {
        /// <summary>
        /// Optional filter for matching facts based on the given conditions.
        /// </summary>
        /// <param name="conditions">Set of conditions the fact must satisfy to trigger the rule.</param>
        /// <returns>Pattern continuation expression builder.</returns>
        IContinuationExpression<TFact> Where(params Expression<Func<TFact, bool>>[] conditions);
    }
}