using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class Tuple : IEnumerable<Fact>
    {
        private readonly List<Tuple> _leftTuples = new List<Tuple>();
        private readonly List<Tuple> _childTuples = new List<Tuple>(); 

        public Tuple(Fact fact)
        {
            RightFact = fact;
            RightFact.ChildTuples.Add(this);
        }

        public Tuple(Tuple left, Fact right) : this(right)
        {
            _leftTuples.AddRange(left.LeftTuples);
            _leftTuples.Add(left);
            left.ChildTuples.Add(this);
        }

        public Fact RightFact { get; private set; }
        public IList<Tuple> LeftTuples { get { return _leftTuples; } }
        public IList<Tuple> ChildTuples { get { return _childTuples; } }

        public void Clear()
        {
            RightFact.ChildTuples.Remove(this);
            RightFact = null;

            Tuple left = LeftTuples.LastOrDefault();
            if (left != null) left.ChildTuples.Remove(this);
            LeftTuples.Clear();

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
                _currentTuple = (_index < 0) ? _rootTuple : _rootTuple.LeftTuples[_index];
                return true;
            }

            public void Reset()
            {
                _currentTuple = null;
                _index = _rootTuple.LeftTuples.Count;
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