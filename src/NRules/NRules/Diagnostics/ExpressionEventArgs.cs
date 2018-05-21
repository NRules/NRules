using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to expression evaluation events.
    /// </summary>
    public class ExpressionEventArgs : EventArgs
    {
        private readonly object[] _arguments;
        private readonly object _argument;

        /// <summary>
        /// Initializes a new instance of the <c>ExpressionEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression that caused the event.</param>
        /// <param name="exception">Exception thrown during expression evaluation.</param>
        /// <param name="argument">Argument passed to expression during evaluation.</param>
        /// <param name="result">Result of expression evaluation.</param>
        public ExpressionEventArgs(Expression expression, Exception exception, object argument, object result)
        {
            _argument = argument;
            Expression = expression;
            Exception = exception;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <c>ExpressionEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression that caused the event.</param>
        /// <param name="exception">Exception thrown during expression evaluation.</param>
        /// <param name="arguments">Arguments passed to expression during evaluation.</param>
        /// <param name="result">Result of expression evaluation.</param>
        public ExpressionEventArgs(Expression expression, Exception exception, object[] arguments, object result)
        {
            _arguments = arguments;
            Expression = expression;
            Exception = exception;
            Result = result;
        }

        /// <summary>
        /// Expression that caused the event;
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Exception thrown during expression evaluation.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Arguments passed to the expression during evaluation.
        /// </summary>
        public IEnumerable<object> Arguments
        {
            get
            {
                if (_arguments != null)
                {
                    foreach (var argument in _arguments)
                    {
                        yield return argument;
                    }
                }
                else
                {
                    yield return _argument;
                }
            }
        }

        /// <summary>
        /// Result of expression evaluation.
        /// </summary>
        public object Result { get; }
    }
}
