using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's right hand side (actions) expression builder.
    /// </summary>
    public interface ILeftHandSide
    {
        /// <summary>
        /// Defines a pattern for facts matching a set of conditions.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="alias">Alias for the matching fact.</param>
        /// <param name="conditions">Set of conditions the fact must satisfy, for the rule to fire.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Match<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that aggregates matching facts into a collection.
        /// </summary>
        /// <typeparam name="T">Type of facts to aggregate.</typeparam>
        /// <param name="alias">Alias for the collection of matching facts.</param>
        /// <param name="itemConditions">Set of conditions the facts must satisfy to get into the collection.
        /// The rule only fires if the collection contains elements.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if there is at least one matching fact.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of conditions the facts must satisfy to be considered.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if there are no matching facts.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of conditions the facts must satisfy to be considered.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Not<T>(params Expression<Func<T, bool>>[] conditions);
    }
}