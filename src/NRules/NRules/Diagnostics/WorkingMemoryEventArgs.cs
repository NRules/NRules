using System;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to working memory events.
    /// </summary>
    public class WorkingMemoryEventArgs : EventArgs
    {
        private readonly Fact _fact;

        internal WorkingMemoryEventArgs(Fact fact)
        {
            _fact = fact;
        }

        /// <summary>
        /// Fact related to the event.
        /// </summary>
        public IFact Fact { get { return _fact; } }
    }
}