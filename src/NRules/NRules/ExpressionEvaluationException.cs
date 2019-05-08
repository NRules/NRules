using System;
using System.Linq.Expressions;

namespace NRules
{
    /// <summary>
    /// Exception raised if evaluation of the expression failed.
    /// Inner exception contains the details of the failure.
    /// </summary>
    internal class ExpressionEvaluationException : Exception
    {
        /// <summary>
        /// Expression that failed to evaluate.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Indicates whether exception was handled via event handler.
        /// </summary>
        public bool IsHandled { get; }

        internal ExpressionEvaluationException(Exception inner, Expression expression, bool isHandled)
            : base("Expression evaluation failed", inner)
        {
            Expression = expression;
            IsHandled = isHandled;
        }
    }
}
