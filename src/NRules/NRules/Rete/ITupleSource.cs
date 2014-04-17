using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface ITupleSource
    {
        IEnumerable<Tuple> GetTuples(IExecutionContext context);
        void Attach(ITupleSink sink);
    }
}