using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Expressions;

namespace NRules.Fluent.Dsl
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Creates a query from matching facts in the engine's working memory.
        /// </summary>
        /// <typeparam name="TFact">Type of facts to query.</typeparam>
        /// <param name="query">Query expression builder.</param>
        /// <param name="conditions">Set of conditions the fact must satisfy.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<TFact> Match<TFact>(this IQuery query, params Expression<Func<TFact, bool>>[] conditions)
        {
            query.Builder.FactQuery(conditions);
            return new QueryExpression<TFact>(query.Builder);
        }

        /// <summary>
        /// Filters source facts using a set of predicate expressions.
        /// The facts must match all predicate expressions in order to pass the filter.
        /// </summary>
        /// <typeparam name="TSource">Type of facts to filter.</typeparam>
        /// <param name="source">Query expression builder.</param>
        /// <param name="predicates">Filter expressions.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<TSource> Where<TSource>(this IQuery<TSource> source, params Expression<Func<TSource, bool>>[] predicates)
        {
            source.Builder.Where(predicates);
            return new QueryExpression<TSource>(source.Builder);
        }

        /// <summary>
        /// Projects source facts using selector expression.
        /// </summary>
        /// <typeparam name="TSource">Type of source facts.</typeparam>
        /// <typeparam name="TResult">Type of projected facts.</typeparam>
        /// <param name="source">Query expression builder.</param>
        /// <param name="selector">Projection expression.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<TResult> Select<TSource, TResult>(this IQuery<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            source.Builder.Select(selector);
            return new QueryExpression<TResult>(source.Builder);
        }

        /// <summary>
        /// Flattens source facts using collection selector expression.
        /// </summary>
        /// <typeparam name="TSource">Type of source facts.</typeparam>
        /// <typeparam name="TResult">Type of flattened facts.</typeparam>
        /// <param name="source">Query expression builder.</param>
        /// <param name="selector">Collection flattening expression.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<TResult> SelectMany<TSource, TResult>(this IQuery<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            source.Builder.SelectMany(selector);
            return new QueryExpression<TResult>(source.Builder);
        }

        /// <summary>
        /// Aggregates source facts into groups based on a grouping key.
        /// </summary>
        /// <typeparam name="TSource">Type of source facts.</typeparam>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <param name="source">Query expression builder.</param>
        /// <param name="keySelector">Key selection expression.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQuery<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            source.Builder.GroupBy(keySelector, x => x);
            return new QueryExpression<IGrouping<TKey, TSource>>(source.Builder);
        }

        /// <summary>
        /// Aggregates source facts into groups based on a grouping key.
        /// Projects facts as part of grouping based on a value selection expression.
        /// </summary>
        /// <typeparam name="TSource">Type of source facts.</typeparam>
        /// <typeparam name="TKey">Type of grouping key.</typeparam>
        /// <typeparam name="TElement">Type of projected facts.</typeparam>
        /// <param name="source">Query expression builder.</param>
        /// <param name="keySelector">Key selection expression.</param>
        /// <param name="elementSelector">Projected fact selection expression.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IQuery<TSource> source, Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector)
        {
            source.Builder.GroupBy(keySelector, elementSelector);
            return new QueryExpression<IGrouping<TKey, TElement>>(source.Builder);
        }

        /// <summary>
        /// Aggregates matching facts into a collection.
        /// </summary>
        /// <typeparam name="TSource">Type of source facts.</typeparam>
        /// <param name="source">Query expression builder.</param>
        /// <returns>Query expression builder.</returns>
        public static IQuery<IEnumerable<TSource>> Collect<TSource>(this IQuery<TSource> source)
        {
            source.Builder.Collect<TSource>();
            return new QueryExpression<IEnumerable<TSource>>(source.Builder);
        }
    }
}