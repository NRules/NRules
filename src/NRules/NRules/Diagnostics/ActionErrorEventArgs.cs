using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during action execution.
    /// </summary>
    public class ActionErrorEventArgs : RecoverableErrorEventArgs
    {
        private readonly IActivation _activation;

        internal ActionErrorEventArgs(Exception exception, Expression expression, IActivation activation) : base(exception)
        {
            Action = expression;
            _activation = activation;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => _activation.Rule;

        /// <summary>
        /// Action that caused exception.
        /// </summary>
        public Expression Action { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => _activation.Facts;
    }
}