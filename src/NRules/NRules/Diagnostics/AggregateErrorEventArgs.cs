using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during aggregate expression evaluation.
    /// </summary>
    public class AggregateErrorEventArgs : AggregateEventArgs, IRecoverableError
    {
        /// <summary>
        /// Initializes a new instance of the <c>AggregateErrorEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        /// <param name="fact">Fact related to the event.</param>
        public AggregateErrorEventArgs(Expression expression, Exception exception, object[] arguments, ITuple tuple, IFact fact)
            : base(expression, exception, arguments, null, tuple, fact)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <c>AggregateErrorEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="argument">Expression argument.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        /// <param name="fact">Fact related to the event.</param>
        public AggregateErrorEventArgs(Expression expression, Exception exception, object argument, ITuple tuple, IFact fact)
            : base(expression, exception, argument, null, tuple, fact)
        {
        }

        public bool IsHandled { get; set; }
    }
}