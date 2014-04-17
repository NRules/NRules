using System.Collections.Generic;
using System.Linq;

namespace NRules.Rete
{
    internal class ObjectInputAdapter : ITupleSink, IObjectSource
    {
        private readonly ITupleSource _source;
        private IObjectSink _sink;

        public ObjectInputAdapter(ITupleSource source)
        {
            _source = source;
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            _sink.PropagateAssert(context, tuple.RightFact);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            _sink.PropagateUpdate(context, tuple.RightFact);
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            _sink.PropagateRetract(context, tuple.RightFact);
        }

        public IEnumerable<Fact> GetFacts(IExecutionContext context)
        {
            return _source.GetTuples(context).Select(t => t.RightFact);
        }

        public void Attach(IObjectSink sink)
        {
            _sink = sink;
        }
    }
}