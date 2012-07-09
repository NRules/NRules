using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class BetaMemory : ITupleSink, ITupleMemory
    {
        private readonly List<Tuple> _tuples = new List<Tuple>();
        private ITupleSink _sink;

        public BetaMemory(ITupleSource source)
        {
            source.Attach(this);
        }

        public void PropagateAssert(Tuple tuple)
        {
            _tuples.Add(tuple);
            _sink.PropagateAssert(tuple);
        }

        public void PropagateRetract(Fact fact)
        {
            var matchingTuples = _tuples.Where(t => t.Elements.Contains(fact)).ToArray();
            foreach (var matchingTuple in matchingTuples)
            {
                _tuples.Remove(matchingTuple);
            }
            _sink.PropagateRetract(fact);
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