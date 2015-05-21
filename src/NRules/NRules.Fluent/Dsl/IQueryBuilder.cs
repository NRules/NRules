using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Internal builder for queries.
    /// </summary>
    public interface IQueryBuilder
    {
        void FactQuery<TSource>();
        void Where<TSource>(Expression<Func<TSource, bool>>[] predicates);
        void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector);
        void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector);
        void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector);
        void Collect<TSource>();
    }
}