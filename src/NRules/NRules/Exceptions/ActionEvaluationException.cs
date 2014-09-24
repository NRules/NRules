using System;
using System.Linq.Expressions;

namespace NRules.Exceptions
{
    public class ActionEvaluationException : ExpressionEvaluationException
    {
        public string Action
        {
            get { return Expression.ToString(); }
        }

        internal ActionEvaluationException(string message, LambdaExpression actionExpression, Exception innerException)
            : base(message, actionExpression, innerException)
        {
        }
    }
}