using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal abstract class AlphaNode : IObjectSink
    {
        protected AlphaNode()
        {
            ChildNodes = new List<AlphaNode>();
        }

        public AlphaMemoryNode MemoryNode { get; set; }
        public IList<AlphaNode> ChildNodes { get; private set; }

        public abstract bool IsSatisfiedBy(Fact fact);

        public void PropagateAssert(IWorkingMemory workingMemory, Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateAssert(workingMemory, fact);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateAssert(workingMemory, fact);
                }
            }
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateUpdate(workingMemory, fact);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateUpdate(workingMemory, fact);
                }
            }
            else
            {
                ForceRetract(workingMemory, fact);
            }
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateRetract(workingMemory, fact);
                }
                if (MemoryNode != null)
                {
                    MemoryNode.PropagateRetract(workingMemory, fact);
                }
            }
        }

        public void ForceRetract(IWorkingMemory workingMemory, Fact fact)
        {
            foreach (var childNode in ChildNodes)
            {
                childNode.ForceRetract(workingMemory, fact);
            }
            if (MemoryNode != null)
            {
                MemoryNode.PropagateRetract(workingMemory, fact);
            }
        }
    }
}