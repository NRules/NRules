using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface ITupleSource
    {
        IEnumerable<Tuple> GetTuples(IWorkingMemory workingMemory);
        void Attach(ITupleSink sink);
    }
}