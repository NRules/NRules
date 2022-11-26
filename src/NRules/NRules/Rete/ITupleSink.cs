using System.Collections.Generic;

namespace NRules.Rete;

internal interface ITupleSink : INode
{
    void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
    void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
    void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
}