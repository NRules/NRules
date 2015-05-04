using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IBetaMemoryNode : ITupleSource, INode
    {
        IEnumerable<ITupleSink> Sinks { get; }
        void PropagateAssert(IExecutionContext context, Tuple tuple, Fact fact);
        void PropagateUpdate(IExecutionContext context, Tuple tuple, Fact fact);
        void PropagateRetract(IExecutionContext context, Tuple tuple, Fact fact);
    }

    internal class BetaMemoryNode : IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public IEnumerable<ITupleSink> Sinks { get { return _sinks; } }

        public void PropagateAssert(IExecutionContext context, Tuple tuple, Fact fact)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var childTuple = new Tuple(tuple, fact);
            foreach (var sink in _sinks)
            {
                sink.PropagateAssert(context, childTuple);
            }
            memory.Add(childTuple);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple, Fact fact)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            Tuple childTuple = memory.FindTuple(tuple, fact);
            if (childTuple == null)
            {
                PropagateAssert(context, tuple, fact);
            }
            else
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateUpdate(context, childTuple);
                }
            }
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple, Fact fact)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            Tuple childTuple = memory.FindTuple(tuple, fact);
            if (childTuple != null)
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateRetract(context, childTuple);
                }
                memory.Remove(childTuple);
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

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBetaMemoryNode(context, this);
        }
    }
}