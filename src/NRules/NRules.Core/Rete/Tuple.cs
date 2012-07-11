using System.Collections;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class Tuple : IEnumerable<Fact>
    {
        public Tuple(Fact fact)
        {
            RightFact = fact;
            RightFact.ChildTuples.Add(this);
            ChildTuples = new List<Tuple>();
        }

        public Tuple(Tuple left, Fact right) : this(right)
        {
            LeftTuple = left;
            LeftTuple.ChildTuples.Add(this);
        }

        public Fact RightFact { get; private set; }
        public Tuple LeftTuple { get; private set; }
        public IList<Tuple> ChildTuples { get; private set; }

        public void Clear()
        {
            RightFact.ChildTuples.Remove(this);
            RightFact = null;
            if (LeftTuple != null) LeftTuple.ChildTuples.Remove(this);
            LeftTuple = null;
            ChildTuples.Clear();
        }

        public IEnumerator<Fact> GetEnumerator()
        {
            var enumerator = new FactEnumerator(this);
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class FactEnumerator : IEnumerator<Fact>
        {
            private Tuple _currentTuple;
            private readonly Tuple _rootTuple;

            public FactEnumerator(Tuple tuple)
            {
                _rootTuple = tuple;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _currentTuple = (_currentTuple == null)
                                    ? _rootTuple
                                    : _currentTuple.LeftTuple;
                return _currentTuple != null;
            }

            public void Reset()
            {
                _currentTuple = null;
            }

            public Fact Current
            {
                get { return _currentTuple.RightFact; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}