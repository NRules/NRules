using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class DummyNode : IBetaMemoryNode
    {
        public void InitializeMemory(IBetaMemory memory)
        {
            memory.Tuples.Add(new Tuple());
        }

        public IEnumerable<Tuple> GetTuples(IWorkingMemory workingMemory)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            return memory.Tuples;
        }

        public void Attach(ITupleSink sink)
        {
            //Do nothing
        }
    }
}