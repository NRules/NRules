using System;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder for optional additional conditions on collection pattern.
    /// </summary>
    /// <typeparam name="TCollection">Type of collection.</typeparam>
    public interface ICollectPattern<TCollection> : ILeftHandSide
    {
        /// <summary>
        /// Optional conditions on the collection pattern.
        /// </summary>
        /// <param name="conditions">Collection conditions.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Where(params Expression<Func<TCollection, bool>>[] conditions);
    }
}