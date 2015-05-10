using System;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder for optional conditions on group by pattern.
    /// </summary>
    /// <typeparam name="TKey">Type of grouping key.</typeparam>
    /// <typeparam name="T">Type of group element.</typeparam>
    public interface IGroupByPatternExpression<TKey, T> : ILeftHandSideExpression
    {
        /// <summary>
        /// Optional conditions on the group by pattern.
        /// </summary>
        /// <param name="conditions">Group conditions.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Where(params Expression<Func<IGrouping<TKey, T>, bool>>[] conditions);
    }
}