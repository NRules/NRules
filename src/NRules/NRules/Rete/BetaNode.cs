using System.Collections.Generic;

namespace NRules.Rete
{
    internal abstract class BetaNode : ITupleSink
    {
        public IBetaMemoryNode MemoryNode { get; set; }

        public abstract void PropagateAssert(IExecutionContext context, IList<Tuple> tuples);
        public abstract void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples);
        public abstract void PropagateRetract(IExecutionContext context, IList<Tuple> tuples);

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}