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
        private readonly IMatch _match;

        /// <summary>
        /// Initializes a new instance of the <c>AgendaEventArgs</c> class.
        /// </summary>
        /// <param name="match">Rule match related to the event.</param>
        public AgendaEventArgs(IMatch match)
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
