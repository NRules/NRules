using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface ITupleSink: INode
    {
        void PropagateAssert(IExecutionContext context, IList<Tuple> tuples);
        void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples);
        void PropagateRetract(IExecutionContext context, IList<Tuple> tuples);
    }
}