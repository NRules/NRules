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
            Sink.PropagateAssert(context, tuple.RightFact);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            Sink.PropagateUpdate(context, tuple.RightFact);
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            Sink.PropagateRetract(context, tuple.RightFact);
        }

        public IEnumerable<Fact> GetFacts(IExecutionContext context)
        {
            return _source.GetTuples(context).Select(t => t.RightFact);
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