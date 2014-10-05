using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IBetaMemoryNode : ITupleSource, ITupleSink
    {
        IEnumerable<ITupleSink> Sinks { get; }
    }

    internal class BetaMemoryNode : IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public IEnumerable<ITupleSink> Sinks { get { return _sinks; } } 

        public void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            foreach (var sink in _sinks)
            {
                sink.PropagateAssert(context, tuple);
            }
            memory.Tuples.Add(tuple);
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
            foreach (var sink in _sinks)
            {
                sink.PropagateRetract(context, tuple);
            }
            memory.Tuples.Remove(tuple);
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

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBetaMemoryNode(context, this);
        }
    }
}