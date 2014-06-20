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

        public FactInfo Fact { get { return new FactInfo(_fact); } }
    }
}