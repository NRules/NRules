using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Exception raised by <see cref="IAggregateExpression"/> if evaluation of the expression failed.
    /// Inner exception contains the details of the failure.
    /// </summary>
    internal class AggregateExpressionException : Exception
    {
        /// <summary>
        /// Expression that failed to evaluate.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Tuple that caused the exception.
        /// </summary>
        public ITuple Tuple { get; }

        /// <summary>
        /// Fact that caused the exception.
        /// </summary>
        public IFact Fact { get; }

        internal AggregateExpressionException(Exception inner, Expression expression, ITuple tuple, IFact fact)
            : base("Aggregate expression evaluation failed", inner)
        {
            Expression = expression;
            Tuple = tuple;
            Fact = fact;
        }
    }
}
