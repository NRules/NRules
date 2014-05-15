using System.Collections.Generic;
using System.Linq;

namespace NRules.Rete
{
    internal class ObjectInputAdapter : ITupleSink, IObjectSource
    {
        private readonly ITupleSource _source;

        public IObjectSink Sink { get; private set; }

        public ObjectInputAdapter(ITupleSource source)
        {
            _source = source;
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            var wrapperFact = new WrapperFact(tuple);
            context.WorkingMemory.SetFact(wrapperFact);
            Sink.PropagateAssert(context, wrapperFact);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            var wrapperFact = context.WorkingMemory.GetFact(tuple);
            Sink.PropagateUpdate(context, wrapperFact);
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            var wrapperFact = context.WorkingMemory.GetFact(tuple);
            Sink.PropagateRetract(context, wrapperFact);
            context.WorkingMemory.RemoveFact(wrapperFact);
        }

        public IEnumerable<Fact> GetFacts(IExecutionContext context)
        {
            return _source.GetTuples(context).Select(t => context.WorkingMemory.GetFact(t));
        }

        public void Attach(IObjectSink sink)
        {
            Sink = sink;
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitObjectInputAdapter(context, this);
        }
    }
}