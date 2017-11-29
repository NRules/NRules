using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to agenda events.
    /// </summary>
    public class AgendaEventArgs : EventArgs
    {
        private readonly Activation _activation;

        internal AgendaEventArgs(Activation activation)
        {
            _activation = activation;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule => _activation.Rule;

        /// <summary>
        /// Facts related to the event.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => _activation.Facts;
    }
}
