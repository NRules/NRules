using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's filters expression builder.
    /// </summary>
    public interface IFilterExpression
    {
        /// <summary>
        /// Configures the engine to filter rule's matches, so that updates are only triggered if given keys changed.
        /// If multiple keys are configured, the match is accepted if any of the keys changed.
        /// </summary>
        /// <param name="keySelectors">Key selector expressions.</param>
        /// <returns>Filters expression builder.</returns>
        IFilterExpression OnChange(params Expression<Func<object>>[] keySelectors);

        /// <summary>
        /// Configures the engine to filter rule's matches, so that updates are only triggered if given key changed.
        /// Comparison is done using the custom equality comparer given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keySelector">Key selector expression.</param>
        /// <param name="comparer">Equality comparer for key</param>
        /// <returns></returns>
        IFilterExpression OnChange<T>(Expression<Func<T>> keySelector, Expression<Func<IEqualityComparer<T>>> comparer) where T : class;

        /// <summary>
        /// Configures the engine to filter rule's matches given a set of predicates.
        /// If multiple predicates are configured, the match is accepted if all predicates are true.
        /// </summary>
        /// <param name="predicates">Predicate expressions.</param>
        /// <returns>Filters expression builder.</returns>
        IFilterExpression Where(params Expression<Func<bool>>[] predicates);
    }
}