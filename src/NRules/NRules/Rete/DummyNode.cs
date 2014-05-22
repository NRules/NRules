using System.Collections.Generic;

namespace NRules.Rete
{
    internal class DummyNode : IBetaMemoryNode, IActivatable, INode
    {
        private ITupleSink _sink;

        public void Activate(IExecutionContext context)
        {
            var tuple = new Tuple(this);

            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            memory.Tuples.Add(tuple);

            _sink.PropagateAssert(context, tuple);
        }

        public IEnumerable<Tuple> GetTuples(IExecutionContext context)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            return memory.Tuples;
        }

        public void Attach(ITupleSink sink)
        {
            _sink = sink;
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitDummyNode(context, this);
        }
    }
}