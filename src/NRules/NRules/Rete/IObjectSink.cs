using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IObjectSink : INode
    {
        void PropagateAssert(IExecutionContext context, List<Fact> facts);
        void PropagateUpdate(IExecutionContext context, List<Fact> facts);
        void PropagateRetract(IExecutionContext context, List<Fact> facts);
    }
}