using System;
using System.Linq.Expressions;

namespace NRules.AgendaFilters
{
    /// <summary>
    /// Exception raised by <see cref="IActivationExpression"/> or <see cref="IActivationExpression"/>
    /// if evaluation of the expression failed.
    /// Inner exception contains the details of the failure.
    /// </summary>
    internal class ActivationExpressionException : Exception
    {
        /// <summary>
        /// Expression that failed to evaluate.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Activation that caused the exception.
        /// </summary>
        public Activation Activation { get; }

        /// <summary>
        /// Indicates whether exception was handled via event handler.
        /// </summary>
        public bool IsHandled { get; }

        internal ActivationExpressionException(Exception inner, Expression expression, Activation activation, bool isHandled)
            : base("Activation expression evaluation failed", inner)
        {
            Expression = expression;
            Activation = activation;
            IsHandled = isHandled;
        }
    }
}
