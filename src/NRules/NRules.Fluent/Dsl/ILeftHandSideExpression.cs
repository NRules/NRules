using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's left hand side (conditions) expression builder.
    /// </summary>
    public interface ILeftHandSideExpression
    {
        /// <summary>
        /// Defines a pattern for facts matching a set of conditions.
        /// Binds matching fact to a variable.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to match.</typeparam>
        /// <param name="alias">Alias for the matching fact.</param>
        /// <param name="conditions">Set of conditions the fact must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Match<TFact>(Expression<Func<TFact>> alias, params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern for facts matching a set of conditions.
        /// Does not bind matching fact to a variable.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of additional conditions the fact must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Match<TFact>(params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that aggregates matching facts into a collection.
        /// </summary>
        /// <typeparam name="TFact">Type of facts to aggregate.</typeparam>
        /// <param name="alias">Alias for the collection of matching facts.</param>
        /// <param name="conditions">Set of conditions the facts must satisfy to get into the collection.</param>
        /// <returns>Expression builder for collection conditions.</returns>
        ICollectPatternExpression<TFact> Collect<TFact>(Expression<Func<IEnumerable<TFact>>> alias, params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that aggregates matching facts into groups.
        /// The rule is fired for each group separately.
        /// </summary>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <typeparam name="TFact">Type of facts to aggregate.</typeparam>
        /// <param name="alias">Alias for the group of matching facts.</param>
        /// <param name="keySelector">Key selector.</param>
        /// <param name="conditions">Set of conditions the facts must satisfy to get into the grouping.</param>
        /// <returns>Expression builder for the group conditions.</returns>
        IGroupByPatternExpression<TKey, TFact> GroupBy<TKey, TFact>(Expression<Func<IGrouping<TKey, TFact>>> alias, Expression<Func<TFact, TKey>> keySelector, params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if there is at least one matching fact (existential quantifier).
        /// </summary>
        /// <typeparam name="TFact">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of conditions the facts must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Exists<TFact>(params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if there are no matching facts (negation quantifier).
        /// </summary>
        /// <typeparam name="TFact">Type of fact to match.</typeparam>
        /// <param name="conditions">Set of conditions the facts must not satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Not<TFact>(params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if all facts that match the base condition
        /// also match all the remaining conditions (universal quantifier).
        /// </summary>
        /// <typeparam name="TFact">Type of fact to match.</typeparam>
        /// <param name="baseCondition">Base condition that filters the facts to match the remaining conditions.</param>
        /// <param name="conditions">Set of additional conditions that all matching facts must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions);

        /// <summary>
        /// Defines a pattern that triggers the rule only if all facts of a given type match the condition.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to match.</typeparam>
        /// <param name="condition">Condition that all facts of a given type must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> condition);

        /// <summary>
        /// Defines a group of patterns joined by an AND operator.
        /// If all of the patterns in the group match then the whole group matches.
        /// </summary>
        /// <param name="builder">Group expression builder.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression And(Action<ILeftHandSideExpression> builder);

        /// <summary>
        /// Defines a group of patterns joined by an OR operator.
        /// If either of the patterns in the group matches then the whole group matches.
        /// </summary>
        /// <param name="builder">Group expression builder.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Or(Action<ILeftHandSideExpression> builder);
    }
}