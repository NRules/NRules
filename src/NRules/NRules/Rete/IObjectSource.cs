using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IObjectSource
    {
        IEnumerable<Fact> GetFacts(IWorkingMemory workingMemory);
        void Attach(IObjectSink sink);
    }
}