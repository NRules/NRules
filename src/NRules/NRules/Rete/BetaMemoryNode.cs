using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IBetaMemoryNode : ITupleSource
    {
        void InitializeMemory(IBetaMemory memory);
    }

    internal class BetaMemoryNode : ITupleSink, IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Tuples.Add(tuple);
            foreach (var sink in _sinks)
            {
                sink.PropagateAssert(workingMemory, tuple);
            }
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            foreach (var sink in _sinks)
            {
                sink.PropagateUpdate(workingMemory, tuple);
            }
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            IBetaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Tuples.Remove(tuple);
            foreach (var sink in _sinks)
            {
                sink.PropagateRetract(workingMemory, tuple);
            }
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
            _sinks.Add(sink);
        }
    }
}