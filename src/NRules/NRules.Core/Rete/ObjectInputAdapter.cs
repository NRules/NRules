using System.Linq;

namespace NRules.Core.Rete
{
    internal class ObjectInputAdapter : ITupleSink
    {
        private readonly IObjectSink _sink;

        public ObjectInputAdapter(IObjectSink sink)
        {
            _sink = sink;
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
    }
}
