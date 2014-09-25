using System;
using System.Linq.Expressions;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating expressions as part of rules execution.
    /// </summary>
    public class RuleExpressionEvaluationException : RuleExecutionException
    {
        /// <summary>
        /// Expression that caused exception.
        /// </summary>
        protected LambdaExpression Expression { get; private set; }

        internal RuleExpressionEvaluationException(string message, LambdaExpression expression, Exception innerException)
            : base(message, innerException)
        {
            Expression = expression;
        }
    }
}