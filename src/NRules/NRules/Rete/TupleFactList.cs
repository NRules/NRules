using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    internal interface ITupleFactEnumerator
    {
        Tuple CurrentTuple { get; }
        Fact CurrentFact { get; }
        bool MoveNext();
        void Reset();
    }

    internal interface ITupleFactList
    {
        ITupleFactEnumerator GetEnumerator();
        int Count { get; }
    }

    [DebuggerDisplay("TupleFactList ({Count})")]
    internal class TupleFactList : ITupleFactList
    {
        private readonly List<Tuple> _tuples = new List<Tuple>(); 
        private readonly List<Fact> _facts = new List<Fact>();

        public int Count { get { return _tuples.Count; } }

        public void Add(Tuple tuple, Fact fact)
        {
            _tuples.Add(tuple);
            _facts.Add(fact);
        }

        private class TupleFactEnumerator : ITupleFactEnumerator
        {
            private readonly IEnumerator<Tuple> _tupleEnumerator;
            private readonly IEnumerator<Fact> _factEnumerator;

            public TupleFactEnumerator(IEnumerator<Tuple> tupleEnumerator, IEnumerator<Fact> factEnumerator)
            {
                _tupleEnumerator = tupleEnumerator;
                _factEnumerator = factEnumerator;
            }

            public Tuple CurrentTuple { get { return _tupleEnumerator.Current; } }
            public Fact CurrentFact { get { return _factEnumerator.Current; } }

            public bool MoveNext()
            {
                bool hasNextTuple = _tupleEnumerator.MoveNext();
                bool hasNextFact = _factEnumerator.MoveNext();
                return hasNextTuple && hasNextFact;
            }

            public void Reset()
            {
                _tupleEnumerator.Reset();
                _factEnumerator.Reset();
            }
        }

        public ITupleFactEnumerator GetEnumerator()
        {
            var tupleEnumerator = _tuples.GetEnumerator();
            var factEnumerator = _facts.GetEnumerator();
            return new TupleFactEnumerator(tupleEnumerator, factEnumerator);
        }
    }
}
