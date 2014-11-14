using System.Collections.Generic;

namespace NRules.Rete
{
    internal class DummyNode : IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public IEnumerable<ITupleSink> Sinks
        {
            get { return _sinks; }
        }

        public void Activate(IExecutionContext context)
        {
            var tuple = new Tuple();

            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            foreach (ITupleSink sink in _sinks)
            {
                sink.PropagateAssert(context, tuple);
            }
            memory.Add(tuple);
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
            visitor.VisitDummyNode(context, this);
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple, Fact fact)
        {
            //Do nothing
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple, Fact fact)
        {
            //Do nothing
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple, Fact fact)
        {
            //Do nothing
        }
    }
}