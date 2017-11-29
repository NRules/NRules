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
        private readonly Activation _activation;

        internal AgendaErrorEventArgs(Exception exception, Expression expression, Activation activation) : base(exception)
        {
            Expression = expression;
            _activation = activation;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => _activation.Rule;

        /// <summary>
        /// Expression that caused exception.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => _activation.Facts;
    }
}
