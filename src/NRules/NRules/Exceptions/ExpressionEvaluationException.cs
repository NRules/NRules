using System;
using System.Linq.Expressions;

namespace NRules.Exceptions
{
    public class ExpressionEvaluationException : ExecutionException
    {
        protected LambdaExpression Expression { get; private set; }

        internal ExpressionEvaluationException(string message, LambdaExpression expression, Exception innerException)
            : base(message, innerException)
        {
            Expression = expression;
        }
    }
}