using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class DummyNode : ITupleMemory
    {
        private readonly IList<Tuple> _tuples = new List<Tuple>();

        public DummyNode()
        {
            _tuples.Add(new Tuple(this));
        }

        public void PropagateAssert(Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateUpdate(Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateRetract(Tuple tuple)
        {
            //Do nothing
        }

        public void Attach(ITupleSink sink)
        {
            //Do nothing
        }

        public IEnumerable<Tuple> GetTuples()
        {
            return _tuples;
        }
    }
}