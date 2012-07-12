using System.Collections;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class Tuple : IEnumerable<Fact>
    {
        private readonly List<Tuple> _leftTuples = new List<Tuple>();
        private readonly List<Tuple> _childTuples = new List<Tuple>();

        public Tuple(Fact fact, ITupleMemory origin)
        {
            RightFact = fact;
            RightFact.ChildTuples.Add(this);
            Origin = origin;
        }

        public Tuple(Tuple left, Fact right, ITupleMemory origin) : this(right, origin)
        {
            _leftTuples.AddRange(left._leftTuples);
            _leftTuples.Add(left);
            LeftTuple = left;
            LeftTuple.ChildTuples.Add(this);
        }

        public Fact RightFact { get; private set; }
        public Tuple LeftTuple { get; private set; }

        public IList<Tuple> ChildTuples
        {
            get { return _childTuples; }
        }

        public ITupleMemory Origin { get; private set; }

        public void Clear()
        {
            RightFact.ChildTuples.Remove(this);
            RightFact = null;

            if (LeftTuple != null) LeftTuple.ChildTuples.Remove(this);
            _leftTuples.Clear();

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
            private int _index;
            private Tuple _currentTuple;
            private readonly Tuple _rootTuple;

            public FactEnumerator(Tuple tuple)
            {
                _rootTuple = tuple;
                Reset();
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_index < 0) return false;
                _index--;
                _currentTuple = (_index < 0) ? _rootTuple : _rootTuple._leftTuples[_index];
                return true;
            }

            public void Reset()
            {
                _currentTuple = null;
                _index = _rootTuple._leftTuples.Count;
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