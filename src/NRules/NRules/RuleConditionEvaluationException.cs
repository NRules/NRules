using System;
using System.Linq.Expressions;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule condition.
    /// </summary>
    public class RuleConditionEvaluationException : RuleExpressionEvaluationException
    {
        /// <summary>
        /// Condition that caused exception.
        /// </summary>
        public string Condition
        {
            get { return Expression.ToString(); }
        }

        internal RuleConditionEvaluationException(string message, LambdaExpression conditionExpression, Exception innerException)
            : base(message, conditionExpression, innerException)
        {
        }
    }
}