using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Dsl;

/// <summary>
/// Internal builder for queries.
/// </summary>
public interface IQueryBuilder
{
    void FactQuery<TSource>(Expression<Func<TSource, bool>>[] conditions)
        where TSource : notnull;
    void From<TSource>(Expression<Func<TSource>> source)
        where TSource : notnull;
    void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        where TSource : notnull;
    void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        where TSource : notnull;
    void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        where TSource : notnull;
    void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        where TSource : notnull 
        where TKey : notnull
        where TElement : notnull;
    void Collect<TSource>()
        where TSource : notnull;
    void ToLookup<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        where TSource : notnull
        where TElement : notnull;
    void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        where TSource : notnull;
    void Aggregate<TSource, TResult>(string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions)
        where TSource : notnull;
    void Aggregate<TSource, TResult>(string name, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, Type customFactoryType)
        where TSource : notnull;
}