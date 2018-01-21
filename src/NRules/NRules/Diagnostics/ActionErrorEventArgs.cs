using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during action execution.
    /// </summary>
    public class ActionErrorEventArgs : ErrorEventArgs
    {
        private readonly IMatch _match;

        /// <summary>
        /// Initializes a new instance of the <c>ActionErrorEventArgs</c> class.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="expression">Action expression related to the event.</param>
        /// <param name="match">Rule match related to the event.</param>
        public ActionErrorEventArgs(Exception exception, Expression expression, IMatch match) : base(exception)
        {
            Action = expression;
            _match = match;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => _match.Rule;

        /// <summary>
        /// Action that caused exception.
        /// </summary>
        public Expression Action { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => _match.Facts;
    }
}