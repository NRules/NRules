using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder for optional conditions on collection pattern.
    /// </summary>
    /// <typeparam name="TFact">Type of fact to collect.</typeparam>
    public interface ICollectPatternExpression<TFact> : ILeftHandSideExpression
    {
        /// <summary>
        /// Optional conditions on the collection pattern.
        /// </summary>
        /// <param name="conditions">Collection conditions.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Where(params Expression<Func<IEnumerable<TFact>, bool>>[] conditions);
    }
}