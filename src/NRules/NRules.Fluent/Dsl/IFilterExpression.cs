using System;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's filters expression builder.
    /// </summary>
    public interface IFilterExpression
    {
        /// <summary>
        /// Configures the engine to filter rule's matches, so that updates are only triggered if a given key changed.
        /// If multiple change filters are configured, the match is accepted if one or more keys changed.
        /// </summary>
        /// <param name="keySelector">Key selector expression.</param>
        /// <returns>Filters expression builder.</returns>
        IFilterExpression OnChange(Expression<Func<object>> keySelector);

        /// <summary>
        /// Configures the engine to filter rule's matches given a predicate.
        /// If multiple predicate filters are configured, the match is accepted if all predicates are true.
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IFilterExpression Where(Expression<Func<bool>> keySelector);
    }
}