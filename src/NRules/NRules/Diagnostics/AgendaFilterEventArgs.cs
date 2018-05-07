using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to events raised during agenda filter evaluation.
    /// </summary>
    public class AgendaFilterEventArgs : ExpressionEventArgs
    {
        private readonly IMatch _match;

        /// <summary>
        /// Initializes a new instance of the <c>AgendaFilterEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="result">Expression result.</param>
        /// <param name="match">Rule match related to the event.</param>
        public AgendaFilterEventArgs(Expression expression, Exception exception, object[] arguments, object result, IMatch match)
            : base(expression, exception, arguments, result)
        {
            _match = match;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => _match.Rule;

        /// <summary>
        /// Facts related to the event.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => _match.Facts;
    }
}