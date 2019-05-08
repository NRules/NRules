using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface ITupleSink : INode
    {
        void PropagateAssert(IExecutionContext context, List<Tuple> tuples);
        void PropagateUpdate(IExecutionContext context, List<Tuple> tuples);
        void PropagateRetract(IExecutionContext context, List<Tuple> tuples);
    }
}