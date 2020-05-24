using System;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during agenda expression evaluation.
    /// </summary>
    public class AgendaExpressionErrorEventArgs : AgendaExpressionEventArgs, IRecoverableError
    {
        /// <summary>
        /// Initializes a new instance of the <c>AgendaExpressionErrorEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="match">Rule match related to the event.</param>
        public AgendaExpressionErrorEventArgs(Expression expression, Exception exception, object[] arguments, IMatch match)
            : base(expression, exception, arguments, null, match)
        {
        }
        
        internal AgendaExpressionErrorEventArgs(Expression expression, Exception exception, IArguments arguments, IMatch match)
            : base(expression, exception, arguments, null, match)
        {
        }

        /// <inheritdoc />
        public bool IsHandled { get; set; }
    }
}
