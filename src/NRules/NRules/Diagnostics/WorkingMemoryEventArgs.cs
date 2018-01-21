using System;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to working memory events.
    /// </summary>
    public class WorkingMemoryEventArgs : EventArgs
    {
        private readonly IFact _fact;

        /// <summary>
        /// Initializes a new instance of the <c>WorkingMemoryEventArgs</c> class.
        /// </summary>
        /// <param name="fact">Fact related to the event.</param>
        public WorkingMemoryEventArgs(IFact fact)
        {
            _fact = fact;
        }

        /// <summary>
        /// Fact related to the event.
        /// </summary>
        public IFact Fact => _fact;
    }
}