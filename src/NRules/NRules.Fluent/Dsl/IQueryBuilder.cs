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
        void FactQuery<TFact>(Expression<Func<TFact>> alias, Expression<Func<TFact, bool>>[] conditions);
        void FactQuery<TSource>(Expression<Func<TSource, bool>>[] conditions);
        void Query<TResult>(Expression<Func<TResult>> alias, Func<IQuery, IQuery<TResult>> queryAction);
        void Query<TSource>(Func<IQuery, IQuery<TSource>> queryAction);
        void From<TSource>(Expression<Func<TSource>> source);
        void Where<TSource>(Expression<Func<TSource, bool>>[] predicates);
        void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector);
        void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector);
        void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector);
        void Collect<TSource>();
        void Aggregate<TSource, TResult>(string name, IDictionary<string, LambdaExpression> expressionMap);
        void Aggregate<TSource, TResult>(string name, IDictionary<string, LambdaExpression> expressionMap, Type customFactoryType);
    }
}