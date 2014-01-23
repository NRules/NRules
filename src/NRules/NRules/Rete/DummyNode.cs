using System.Collections.Generic;

namespace NRules.Rete
{
    internal class DummyNode : IBetaMemoryNode, IActivatable
    {
        private ITupleSink _sink;

        public void Activate(IWorkingMemory workingMemory)
        {
            var tuple = new Tuple();

            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Tuples.Add(tuple);

            _sink.PropagateAssert(workingMemory, tuple);
        }

        public IEnumerable<Tuple> GetTuples(IWorkingMemory workingMemory)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            return memory.Tuples;
        }

        public void Attach(ITupleSink sink)
        {
            _sink = sink;
        }
    }
}