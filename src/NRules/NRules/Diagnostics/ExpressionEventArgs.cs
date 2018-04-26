using System;
using System.Linq.Expressions;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to expression evaluation events.
    /// </summary>
    public class ExpressionEventArgs : EventArgs
    {
        public ExpressionEventArgs(Expression expression, Exception exception, object[] arguments, object result)
        {
            Expression = expression;
            Exception = exception;
            Arguments = arguments;
            Result = result;
        }

        public Expression Expression { get; }
        public Exception Exception { get; }
        public object[] Arguments { get; }
        public object Result { get; }
    }
}
