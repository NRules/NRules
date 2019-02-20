using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Internal builder for queries.
    /// </summary>
    public interface IQueryBuilder
    {
        void FactQuery<TSource>(Expression<Func<TSource, bool>>[] conditions);
        void From<TSource>(Expression<Func<TSource>> source);
        void Where<TSource>(Expression<Func<TSource, bool>>[] predicates);
        void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector);
        void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector);
        void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector);
        void Collect<TSource>();
        void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection);
        void Aggregate<TSource, TResult>(string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions);
        void Aggregate<TSource, TResult>(string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, Type customFactoryType);
    }
}