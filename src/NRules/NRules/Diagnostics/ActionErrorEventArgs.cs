using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during action evaluation.
    /// </summary>
    public class ActionErrorEventArgs : ActionEventArgs, IRecoverableError
    {
        /// <summary>
        /// Initializes a new instance of the <c>ActionErrorEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="match">Rule match related to the event.</param>
        public ActionErrorEventArgs(Expression expression, Exception exception, object[] arguments, IMatch match)
            : base(expression, exception, arguments, match)
        {
        }

        /// <inheritdoc />
        public bool IsHandled { get; set; }
    }
}