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

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateAssert(workingMemory, tuple.RightFact);
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateUpdate(workingMemory, tuple.RightFact);
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateRetract(workingMemory, tuple.RightFact);
        }

        public IEnumerable<Fact> GetFacts(IWorkingMemory workingMemory)
        {
            return _source.GetTuples(workingMemory).Select(t => t.RightFact);
        }

        public void Attach(IObjectSink sink)
        {
            _sink = sink;
        }
    }
}