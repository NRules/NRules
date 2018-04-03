using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during agenda operation.
    /// </summary>
    public class AgendaErrorEventArgs : ErrorEventArgs
    {
        private readonly IMatch _match;

        /// <summary>
        /// Initializes a new instance of the <c>AgendaErrorEventArgs</c> class.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="match">Rule match related to the event.</param>
        public AgendaErrorEventArgs(Exception exception, Expression expression, IMatch match) : base(exception)
        {
            Expression = expression;
            _match = match;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => _match.Rule;

        /// <summary>
        /// Expression that caused exception.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => _match.Facts;
    }
}
