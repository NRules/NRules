using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class BetaMemory : ITupleMemory
    {
        private readonly List<Tuple> _tuples = new List<Tuple>();
        private ITupleSink _sink;

        public void PropagateAssert(Tuple tuple)
        {
            _tuples.Add(tuple);
            _sink.PropagateAssert(tuple);
        }

        public void PropagateUpdate(Tuple tuple)
        {
            _sink.PropagateUpdate(tuple);
        }

        public void PropagateRetract(Tuple tuple)
        {
            _tuples.Remove(tuple);
            _sink.PropagateRetract(tuple);
        }

        public IEnumerable<Tuple> GetTuples()
        {
            return _tuples;
        }

        public void Attach(ITupleSink sink)
        {
            _sink = sink;
        }
    }
}