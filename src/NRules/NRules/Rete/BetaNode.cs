using System.Collections.Generic;

namespace NRules.Rete
{
    internal abstract class BetaNode : ITupleSink
    {
        protected BetaNode()
        {
            NodeInfo = new NodeDebugInfo();
        }

        public NodeDebugInfo NodeInfo { get; }
        public IBetaMemoryNode MemoryNode { get; set; }

        public abstract void PropagateAssert(IExecutionContext context, List<Tuple> tuples);
        public abstract void PropagateUpdate(IExecutionContext context, List<Tuple> tuples);
        public abstract void PropagateRetract(IExecutionContext context, List<Tuple> tuples);

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}