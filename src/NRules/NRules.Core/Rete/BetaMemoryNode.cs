using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface IBetaMemoryNode : ITupleSource
    {
        void InitializeMemory(IBetaMemory memory);
    }

    internal class BetaMemoryNode : ITupleSink, IBetaMemoryNode
    {
        private ITupleSink _sink;

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Tuples.Add(tuple);
            _sink.PropagateAssert(workingMemory, tuple);
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateUpdate(workingMemory, tuple);
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Tuples.Remove(tuple);
            _sink.PropagateRetract(workingMemory, tuple);
        }

        public void InitializeMemory(IBetaMemory memory)
        {
            //Do nothing
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