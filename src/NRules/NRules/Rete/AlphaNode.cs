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

        public abstract bool IsSatisfiedBy(Fact fact);

        public void PropagateAssert(IExecutionContext context, Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateAssert(context, fact);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateAssert(context, fact);
                }
            }
        }

        public void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateUpdate(context, fact);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateUpdate(context, fact);
                }
            }
            else
            {
                UnsatisfiedFactUpdate(context, fact);
            }
        }

        protected virtual void UnsatisfiedFactUpdate(IExecutionContext context, Fact fact)
        {
            ForceRetract(context, fact);
        }

        public void PropagateRetract(IExecutionContext context, Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateRetract(context, fact);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateRetract(context, fact);
                }
            }
        }

        public void ForceRetract(IExecutionContext context, Fact fact)
        {
            foreach (var childNode in ChildNodes)
            {
                childNode.ForceRetract(context, fact);
            }
            if (MemoryNode != null)
            {
                MemoryNode.PropagateRetract(context, fact);
            }
        }
    }
}