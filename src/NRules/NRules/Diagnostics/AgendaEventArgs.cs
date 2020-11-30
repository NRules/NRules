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
        /// <summary>
        /// Initializes a new instance of the <c>AgendaEventArgs</c> class.
        /// </summary>
        /// <param name="match">Rule match related to the event.</param>
        public AgendaEventArgs(IMatch match)
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
