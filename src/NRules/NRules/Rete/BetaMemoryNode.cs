using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IBetaMemoryNode : ITupleSource
    {
    }

    internal class BetaMemoryNode : ITupleSink, IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            memory.Tuples.Add(tuple);
            foreach (var sink in _sinks)
            {
                sink.PropagateAssert(context, tuple);
            }
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            foreach (var sink in _sinks)
            {
                sink.PropagateUpdate(context, tuple);
            }
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            memory.Tuples.Remove(tuple);
            foreach (var sink in _sinks)
            {
                sink.PropagateRetract(context, tuple);
            }
        }

        public IEnumerable<Tuple> GetTuples(IExecutionContext context)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            return memory.Tuples;
        }

        public void Attach(ITupleSink sink)
        {
            _sinks.Add(sink);
        }
    }
}