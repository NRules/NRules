using System;
using System.Linq.Expressions;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule action.
    /// </summary>
    public class RuleActionEvaluationException : RuleExpressionEvaluationException
    {
        /// <summary>
        /// Action that caused exception.
        /// </summary>
        public string Action
        {
            get { return Expression.ToString(); }
        }

        internal RuleActionEvaluationException(string message, LambdaExpression actionExpression, Exception innerException)
            : base(message, actionExpression, innerException)
        {
        }
    }
}