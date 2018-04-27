using System;
using System.Linq.Expressions;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to expression evaluation events.
    /// </summary>
    public class ExpressionEventArgs : EventArgs
    {
        private readonly object _argument;
        private readonly object[] _arguments;

        public ExpressionEventArgs(Expression expression, Exception exception, object[] arguments, object result)
        {
            _arguments = arguments;
            Expression = expression;
            Exception = exception;
            Result = result;
        }

        public ExpressionEventArgs(Expression expression, Exception exception, object argument, object result)
        {
            _argument = argument;
            Expression = expression;
            Exception = exception;
            Result = result;
        }

        public Expression Expression { get; }
        public Exception Exception { get; }
        public object[] Arguments => _arguments ?? new[] {_argument};
        public object Result { get; }
    }
}
