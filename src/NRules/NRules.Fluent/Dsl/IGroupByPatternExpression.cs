using System;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder for optional conditions on group by pattern.
    /// </summary>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="TFact">Type of fact to group.</typeparam>
    public interface IGroupByPatternExpression<TKey, TFact> : ILeftHandSideExpression
    {
        /// <summary>
        /// Optional conditions on the group by pattern.
        /// </summary>
        /// <param name="conditions">Group conditions.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Where(params Expression<Func<IGrouping<TKey, TFact>, bool>>[] conditions);
    }
}