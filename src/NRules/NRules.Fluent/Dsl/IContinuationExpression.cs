using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Expression builder to continue defining a pattern.
    /// </summary>
    /// <typeparam name="TFact">Type of fact that the pattern is for.</typeparam>
    public interface IContinuationExpression<TFact> : ILeftHandSideExpression
    {
        /// <summary>
        /// Projects matching facts using given expression.
        /// </summary>
        /// <param name="alias">Alias for the projected fact.</param>
        /// <param name="selector">Projection expression.</param>
        /// <returns>Expression builder to continue building a pattern.</returns>
        ILeftHandSideExpression Select<TResult>(Expression<Func<TResult>> alias, Expression<Func<TFact, TResult>> selector);
        
        /// <summary>
        /// Projects matching facts using given expression.
        /// </summary>
        /// <param name="selector">Projection expression.</param>
        /// <returns>Expression builder to continue building a pattern.</returns>
        IContinuationExpression<TResult> Select<TResult>(Expression<Func<TFact, TResult>> selector);
        
        /// <summary>
        /// Aggregates matching facts into a collection.
        /// </summary>
        /// <param name="alias">Alias for the collection of matching facts.</param>
        /// <returns>Expression builder for collection conditions.</returns>
        IConditionExpression<IEnumerable<TFact>> Collect(Expression<Func<IEnumerable<TFact>>> alias);

        /// <summary>
        /// Aggregates matching facts into groups based on a grouping key.
        /// </summary>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <param name="alias">Alias for the group of matching facts.</param>
        /// <param name="keySelector">Key selector.</param>
        /// <returns>Expression builder for the group conditions.</returns>
        IConditionExpression<IGrouping<TKey, TFact>> GroupBy<TKey>(Expression<Func<IGrouping<TKey, TFact>>> alias, Expression<Func<TFact, TKey>> keySelector);

        /// <summary>
        /// Aggregates matching facts into groups, based on the grouping key.
        /// </summary>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <param name="keySelector">Key selector.</param>
        /// <returns>Expression builder for the continuation of the group pattern.</returns>
        IContinuationConditionExpression<IGrouping<TKey, TFact>> GroupBy<TKey>(Expression<Func<TFact, TKey>> keySelector);

        /// <summary>
        /// Aggregates matching facts into groups, based on the grouping key.
        /// </summary>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <typeparam name="TValue">Type of value to group.</typeparam>
        /// <param name="keySelector">Key selector.</param>
        /// <param name="valueSelector">Value selector.</param>
        /// <returns>Expression builder for the continuation of the group pattern.</returns>
        IContinuationConditionExpression<IGrouping<TKey, TValue>> GroupBy<TKey, TValue>(Expression<Func<TFact, TKey>> keySelector, Expression<Func<TFact, TValue>> valueSelector);
    }
}