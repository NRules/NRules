using System;
using System.Linq.Expressions;

namespace NRules.Exceptions
{
    public class ConditionEvaluationException : ExpressionEvaluationException
    {
        public string Condition
        {
            get { return Expression.ToString(); }
        }

        internal ConditionEvaluationException(string message, LambdaExpression conditionExpression, Exception innerException)
            : base(message, conditionExpression, innerException)
        {
        }
    }
}