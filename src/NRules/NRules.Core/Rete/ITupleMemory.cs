using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface ITupleMemory : ITupleSource
    {
        IEnumerable<Tuple> GetTuples();
    }
}