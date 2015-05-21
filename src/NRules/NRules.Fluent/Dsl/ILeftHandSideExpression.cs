using System;
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
        /// Does not bind matching fact to a variable. Optionally, enables aggregation of matching facts.
        /// </summary>
        /// <param name="conditions">Set of additional conditions the fact must satisfy to trigger the rule.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Match<TFact>(params Expression<Func<TFact, bool>>[] conditions);

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
        /// Queries rules engine for matching facts.
        /// </summary>
        /// <typeparam name="TResult">Query result type.</typeparam>
        /// <param name="alias">Alias for the query results.</param>
        /// <param name="queryExpression">Query expression.</param>
        /// <returns>Left hand side expression builder.</returns>
        ILeftHandSideExpression Query<TResult>(Expression<Func<TResult>> alias, Func<IQuery, IQuery<TResult>> queryExpression);

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