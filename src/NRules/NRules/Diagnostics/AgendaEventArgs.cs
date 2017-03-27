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
        private readonly IActivation _activation;

        internal AgendaEventArgs(IActivation activation)
        {
            _activation = activation;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule { get { return _activation.Rule; } }

        /// <summary>
        /// Facts related to the event.
        /// </summary>
        public IEnumerable<IFactMatch> Facts
        {
            get { return _activation.Facts; }
        }
    }
}
