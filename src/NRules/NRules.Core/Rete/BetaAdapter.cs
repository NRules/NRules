using System.Linq;

namespace NRules.Core.Rete
{
    internal class BetaAdapter : IObjectSink, ITupleSource
    {
        private ITupleMemory _memory;

        public BetaAdapter(IObjectSource source)
        {
            source.Attach(this);
        }

        public void Attach(ITupleMemory sink)
        {
            _memory = sink;
        }

        public void PropagateAssert(Fact fact)
        {
            var tuple = new Tuple(fact, _memory);
            _memory.PropagateAssert(tuple);
        }

        public void PropagateUpdate(Fact fact)
        {
            var childTuples = fact.ChildTuples.Where(t => t.Origin == _memory).ToList();
            foreach (var childTuple in childTuples)
            {
                _memory.PropagateUpdate(childTuple);
            }
        }

        public void PropagateRetract(Fact fact)
        {
            var childTuples = fact.ChildTuples.Where(t => t.Origin == _memory).ToList();
            foreach (var childTuple in childTuples)
            {
                _memory.PropagateRetract(childTuple);
            }
        }
    }
}