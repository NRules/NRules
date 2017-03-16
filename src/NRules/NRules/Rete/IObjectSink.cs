using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IObjectSink : INode
    {
        void PropagateAssert(IExecutionContext context, IList<Fact> facts);
        void PropagateUpdate(IExecutionContext context, IList<Fact> facts);
        void PropagateRetract(IExecutionContext context, IList<Fact> facts);
    }
}