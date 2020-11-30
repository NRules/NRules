using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to events raised during agenda expression evaluation.
    /// </summary>
    public class AgendaExpressionEventArgs : ExpressionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <c>AgendaExpressionEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="result">Expression result.</param>
        /// <param name="match">Rule match related to the event.</param>
        public AgendaExpressionEventArgs(Expression expression, Exception exception, object[] arguments, object result, IMatch match)
            : base(expression, exception, arguments, result)
        {
            Match = match;
        }
        
        internal AgendaExpressionEventArgs(Expression expression, Exception exception, IArguments arguments, object result, IMatch match)
            : base(expression, exception, arguments, result)
        {
            Match = match;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => Match.Rule;

        /// <summary>
        /// Facts related to the event.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => Match.Facts;

        /// <summary>
        /// Rule match related to the event.
        /// </summary>
        public IMatch Match { get; }
    }
}