using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("TupleFactList ({Count})")]
    internal class TupleFactList
    {
        private readonly List<Tuple> _tuples = new List<Tuple>(); 
        private readonly List<Fact> _facts = new List<Fact>();

        public int Count => _tuples.Count;

        public void Add(Tuple tuple, Fact fact)
        {
            _tuples.Add(tuple);
            _facts.Add(fact);
        }

        public struct Enumerator
        {
            private List<Tuple>.Enumerator _tupleEnumerator;
            private List<Fact>.Enumerator _factEnumerator;

            public Enumerator(List<Tuple>.Enumerator tupleEnumerator, List<Fact>.Enumerator factEnumerator)
            {
                _tupleEnumerator = tupleEnumerator;
                _factEnumerator = factEnumerator;
            }

            public Tuple CurrentTuple => _tupleEnumerator.Current;
            public Fact CurrentFact => _factEnumerator.Current;

            public bool MoveNext()
            {
                bool hasNextTuple = _tupleEnumerator.MoveNext();
                bool hasNextFact = _factEnumerator.MoveNext();
                return hasNextTuple && hasNextFact;
            }

            public void Reset()
            {
                ((IEnumerator)_tupleEnumerator).Reset();
                ((IEnumerator)_factEnumerator).Reset();
            }
        }

        public Enumerator GetEnumerator()
        {
            var tupleEnumerator = _tuples.GetEnumerator();
            var factEnumerator = _facts.GetEnumerator();
            return new Enumerator(tupleEnumerator, factEnumerator);
        }
    }
}
