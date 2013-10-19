using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class ObjectInputAdapter : ITupleSink, IObjectSource
    {
        private readonly ITupleSource _source;
        private IObjectSink _sink;

        public ObjectInputAdapter(ITupleSource source)
        {
            _source = source;
        }

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateAssert(workingMemory, tuple.First());
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateUpdate(workingMemory, tuple.First());
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            _sink.PropagateRetract(workingMemory, tuple.First());
        }

        public IEnumerable<Fact> GetFacts(IWorkingMemory workingMemory)
        {
            return _source.GetTuples(workingMemory).Select(t => t.First());
        }

        public void Attach(IObjectSink sink)
        {
            _sink = sink;
        }
    }
}
