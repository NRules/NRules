using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during binding expression evaluation.
    /// </summary>
    public class BindingErrorEventArgs : BindingEventArgs, IRecoverableError
    {
        /// <summary>
        /// Initializes a new instance of the <c>BindingErrorEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        public BindingErrorEventArgs(Expression expression, Exception exception, object[] arguments, ITuple tuple) 
            : base(expression, exception, arguments, null, tuple)
        {
        }

        public bool IsHandled { get; set; }
    }
}