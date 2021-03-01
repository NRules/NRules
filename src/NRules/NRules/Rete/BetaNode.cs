using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete
{
    internal abstract class BetaNode : ITupleSink
    {
        public int Id { get; set; }
        public NodeInfo NodeInfo { get; } = new NodeInfo();
        public BetaMemoryNode MemoryNode { get; set; }

        public abstract void PropagateAssert(IExecutionContext context, List<Tuple> tuples);
        public abstract void PropagateUpdate(IExecutionContext context, List<Tuple> tuples);
        public abstract void PropagateRetract(IExecutionContext context, List<Tuple> tuples);

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}