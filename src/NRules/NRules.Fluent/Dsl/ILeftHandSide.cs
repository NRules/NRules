using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's left hand side (conditions) expression builder.
    /// </summary>
    public interface ILeftHandSide
    {
        /// <summary>
        /// Defines a pattern for facts matching a set of conditions.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="alias">Alias for the matching fact.</param>
        /// <param name="conditions">Set of conditions the fact must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Match<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern for facts matching a set of conditions.
        /// Does not bind matching fact to a variable.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="condition">Condition the fact must satisfy to trigger the rule.</param>
        /// <param name="conditions">Set of additional conditions the fact must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Match<T>(Expression<Func<T, bool>> condition, params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern matching all facts of a given type.
        /// Does not bind matching fact to a variable.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Match<T>();

        /// <summary>
        /// Defines a pattern that aggregates matching facts into a collection.
        /// The rule only fires if the collection contains elements.
        /// </summary>
        /// <typeparam name="T">Type of facts to aggregate.</typeparam>
        /// <param name="alias">Alias for the collection of matching facts.</param>
        /// <param name="itemConditions">Set of conditions the facts must satisfy to get into the collection.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if there is at least one matching fact (existential quantifier).
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of conditions the facts must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if there are no matching facts (negation quantifier).
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of conditions the facts must not satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Not<T>(params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if all facts that match the base condition
        /// also match all the remaining conditions (universal quantifier).
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="baseCondition">Base condition that filters the facts to match the remaining conditions.</param>
        /// <param name="conditions">Set of additional conditions that all matching facts must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide All<T>(Expression<Func<T, bool>> baseCondition, params Expression<Func<T, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if all facts of a given type match the condition.
        /// </summary>
        /// <typeparam name="T">Type of fact to match.</typeparam>
        /// <param name="condition">Condition that all facts of a given type must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide All<T>(Expression<Func<T, bool>> condition);

        /// <summary>
        /// Defines a group of patterns joined by an AND operator.
        /// If all of the patterns in the group match then the whole group matches.
        /// </summary>
        /// <param name="builder">Group expression builder.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide And(Action<ILeftHandSide> builder);

        /// <summary>
        /// Defines a group of patterns joined by an OR operator.
        /// If either of the patterns in the group matches then the whole group matches.
        /// </summary>
        /// <param name="builder">Group expression builder.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSide Or(Action<ILeftHandSide> builder);
    }
}