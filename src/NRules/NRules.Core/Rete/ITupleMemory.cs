using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface ITupleMemory : ITupleSink
    {
        void Attach(ITupleSink sink);
        IEnumerable<Tuple> GetTuples();
    }
}