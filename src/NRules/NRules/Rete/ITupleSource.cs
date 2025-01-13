using System.Collections.Generic;

namespace NRules.Rete;

internal interface ITupleSource
{
    IReadOnlyCollection<Tuple> GetTuples(IExecutionContext context);
    void Attach(ITupleSink sink);
}