using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    internal abstract class AlphaNode : IObjectSink
    {
        protected AlphaNode()
        {
            ChildNodes = new List<AlphaNode>();
            NodeInfo = new NodeDebugInfo();
        }

        public NodeDebugInfo NodeInfo { get; }
        public AlphaMemoryNode MemoryNode { get; set; }

        [DebuggerDisplay("Count = {ChildNodes.Count}")]
        public IList<AlphaNode> ChildNodes { get; }

        public abstract bool IsSatisfiedBy(IExecutionContext context, Fact fact);

        public virtual void PropagateAssert(IExecutionContext context, List<Fact> facts)
        {
            var toAssert = new List<Fact>();
            foreach (var fact in facts)
            {
                if (IsSatisfiedBy(context, fact))
                    toAssert.Add(fact);
            }

            if (toAssert.Count > 0)
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateAssert(context, toAssert);
                }
                MemoryNode?.PropagateAssert(context, toAssert);
            }
        }

        public virtual void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            var toUpdate = new List<Fact>();
            var toRetract = new List<Fact>();
            foreach (var fact in facts)
            {
                if (IsSatisfiedBy(context, fact))
                    toUpdate.Add(fact);
                else
                    toRetract.Add(fact);
            }
            PropagateUpdateInternal(context, toUpdate);
            PropagateRetractInternal(context, toRetract);
        }

        public virtual void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            PropagateRetractInternal(context, facts);
        }

        protected void PropagateUpdateInternal(IExecutionContext context, List<Fact> facts)
        {
            if (facts.Count == 0) return;
            foreach (var childNode in ChildNodes)
            {
                childNode.PropagateUpdate(context, facts);
            }
            MemoryNode?.PropagateUpdate(context, facts);
        }

        protected void PropagateRetractInternal(IExecutionContext context, List<Fact> facts)
        {
            if (facts.Count == 0) return;
            foreach (var childNode in ChildNodes)
            {
                childNode.PropagateRetract(context, facts);
            }
            MemoryNode?.PropagateRetract(context, facts);
        }

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}