using System;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during right-hand side expression evaluation.
    /// </summary>
    public class RhsExpressionErrorEventArgs : RhsExpressionEventArgs, IRecoverableError
    {
        /// <summary>
        /// Initializes a new instance of the <c>RhsExpressionErrorEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="match">Rule match related to the event.</param>
        public RhsExpressionErrorEventArgs(Expression expression, Exception exception, object[] arguments, IMatch match)
            : base(expression, exception, arguments, match)
        {
        }
        
        internal RhsExpressionErrorEventArgs(Expression expression, Exception exception, IArguments arguments, IMatch match)
            : base(expression, exception, arguments, match)
        {
        }

        /// <inheritdoc />
        public bool IsHandled { get; set; }
    }
}