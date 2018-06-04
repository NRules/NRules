using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to events raised during right-hand side expression evaluation.
    /// </summary>
    public class RhsExpressionEventArgs : ExpressionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <c>RhsExpressionEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="match">Rule match related to the event.</param>
        public RhsExpressionEventArgs(Expression expression, Exception exception, object[] arguments, IMatch match)
            : base(expression, exception, arguments, null)
        {
            Match = match;
        }

        /// <summary>
        /// Rule match related to the event.
        /// </summary>
        public IMatch Match { get; }
    }
}