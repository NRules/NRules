using System.Collections.Generic;
using System.Linq;

namespace NRules.Aggregators
{
    /// <summary>
    /// Extension methods used for working with collections of aggregate expressions.
    /// </summary>
    public static class AggregateExpressionExtensions
    {
        /// <summary>
        /// Get an enumerable of matching aggregate expressions.
        /// </summary>
        /// <param name="expressions">The list of aggregate expressions to search through.</param>
        /// <param name="name">Name of the aggregate expressions to find.</param>
        /// <returns></returns>
        public static IEnumerable<IAggregateExpression> Find(this IEnumerable<IAggregateExpression> expressions, string name)
        {
            return expressions.Where(e => e.Name == name);
        }

        /// <summary>
        /// Get a single matching aggregate expression.
        /// </summary>
        /// <param name="expressions">The list of aggregate expressions to search through.</param>
        /// <param name="name">Name of the aggregate expression to find.</param>
        /// <returns></returns>
        public static IAggregateExpression FindSingle(this IEnumerable<IAggregateExpression> expressions, string name)
        {
            return expressions.Find(name).Single();
        }
    }
}