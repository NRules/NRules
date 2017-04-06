using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    internal abstract class AlphaNode : IObjectSink
    {
        protected AlphaNode()
        {
            ChildNodes = new List<AlphaNode>();
        }

        public AlphaMemoryNode MemoryNode { get; set; }

        [DebuggerDisplay("Count = {ChildNodes.Count}")]
        public IList<AlphaNode> ChildNodes { get; private set; }

        public abstract bool IsSatisfiedBy(IExecutionContext context, Fact fact);

        public void PropagateAssert(IExecutionContext context, IList<Fact> facts)
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
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateAssert(context, toAssert);
                }
            }
        }

        public void PropagateUpdate(IExecutionContext context, IList<Fact> facts)
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

            if (toUpdate.Count > 0)
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateUpdate(context, toUpdate);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateUpdate(context, toUpdate);
                }
            }
            if (toRetract.Count > 0)
            {
                UnsatisfiedFactUpdate(context, toRetract);
            }
        }

        public void PropagateRetract(IExecutionContext context, IList<Fact> facts)
        {
            foreach (var childNode in ChildNodes)
            {
                childNode.PropagateRetract(context, facts);
            }
            if (MemoryNode != null)
            {
                MemoryNode.PropagateRetract(context, facts);
            }
        }

        protected virtual void UnsatisfiedFactUpdate(IExecutionContext context, IList<Fact> facts)
        {
            PropagateRetract(context, facts);
        }

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}