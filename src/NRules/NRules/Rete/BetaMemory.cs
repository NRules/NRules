using System.Collections.Generic;
using NRules.Collections;

namespace NRules.Rete
{
    internal interface IBetaMemory
    {
        IEnumerable<Tuple> Tuples { get; }
        bool Contains(Tuple tuple);
        void Add(Tuple tuple);
        void Remove(Tuple tuple);
    }

    internal class BetaMemory : IBetaMemory
    {
        private readonly OrderedHashSet<Tuple> _tuples = new OrderedHashSet<Tuple>();

        public IEnumerable<Tuple> Tuples
        {
            get { return _tuples; }
        }

        public bool Contains(Tuple tuple)
        {
            return _tuples.Contains(tuple);
        }

        public void Add(Tuple tuple)
        {
            _tuples.Add(tuple);
        }

        public void Remove(Tuple tuple)
        {
            _tuples.Remove(tuple);
        }
    }
}