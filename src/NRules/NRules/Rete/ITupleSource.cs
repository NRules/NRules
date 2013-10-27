using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface ITupleSource
    {
        IEnumerable<Tuple> GetTuples(IWorkingMemory workingMemory);
        void Attach(ITupleSink sink);
    }
}