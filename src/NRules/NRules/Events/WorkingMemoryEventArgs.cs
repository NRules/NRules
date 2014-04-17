using System;
using NRules.Rete;

namespace NRules.Events
{
    public class WorkingMemoryEventArgs : EventArgs
    {
        private readonly Fact _fact;

        internal WorkingMemoryEventArgs(Fact fact)
        {
            _fact = fact;
        }

        public object Fact { get { return _fact.Object; } }
    }
}