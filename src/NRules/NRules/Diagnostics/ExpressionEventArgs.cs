using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to expression evaluation events.
    /// </summary>
    public class ExpressionEventArgs : EventArgs
    {
        private readonly object[] _arguments;
        private readonly IArguments _lazyArguments;

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
        
        internal ExpressionEventArgs(Expression expression, Exception exception, IArguments arguments, object result)
        {
            _lazyArguments = arguments;
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
        public virtual IEnumerable<object> Arguments => _lazyArguments?.GetValues() ?? _arguments;

        /// <summary>
        /// Result of expression evaluation.
        /// </summary>
        public object Result { get; }
    }
}
