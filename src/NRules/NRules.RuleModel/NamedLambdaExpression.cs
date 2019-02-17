using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Lambda Expression with a name used in constructing/building an aggregator.
    /// </summary>
    public class NamedLambdaExpression : INamedExpression<LambdaExpression>
    {
        public NamedLambdaExpression(string name, LambdaExpression expression) 
        {
            Name = name;
            Expression = expression;
        }

        /// <summary>
        /// Expression name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Expression.
        /// </summary>
        public LambdaExpression Expression { get; }
    }

    /// <summary>
    /// Expression with a name used in constructing/building aggregators.
    /// </summary>
    public interface INamedExpression<T>
    {
        /// <summary>
        /// Expression name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Expression.
        /// </summary>
        T Expression { get; }
    }

    /// <summary>
    /// Extension methods used for working with collections of named expressions
    /// </summary>
    public static class NamedExpressionExtensions
    {
        /// <summary>
        /// Get an enumerable of matching expressions.
        /// </summary>
        /// <typeparam name="T">The type of expressions to get.</typeparam>
        /// <param name="expressions">The list of expressions to search through.</param>
        /// <param name="name">Name of the expressions to find.</param>
        /// <returns></returns>
        public static IEnumerable<T> Find<T>(this IEnumerable<INamedExpression<T>> expressions, string name)
        {
            return expressions.Where(e => e.Name == name).Select(e => e.Expression);
        }

        /// <summary>
        /// Get a single matching expression.
        /// </summary>
        /// <typeparam name="T">The type of expression to get.</typeparam>
        /// <param name="expressions">The list of expressions to search through.</param>
        /// <param name="name">Name of the expression to find.</param>
        /// <returns></returns>
        public static T FindSingle<T>(this IEnumerable<INamedExpression<T>> expressions, string name)
        {
            return expressions.Find(name).Single();
        }
    }
}