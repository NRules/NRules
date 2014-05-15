using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Rete
{
    internal class Tuple : IEnumerable<Fact>
    {
        private readonly List<Tuple> _leftTuples = new List<Tuple>();
        private readonly List<Tuple> _childTuples = new List<Tuple>();
        private object _state;

        public Tuple()
        {
            Count = 0;
        }

        public Tuple(Tuple left, Fact right)
        {
            Count = left.Count + 1;
            RightFact = right;
            RightFact.ChildTuples.Add(this);
            _leftTuples.AddRange(left._leftTuples);
            _leftTuples.Add(left);
            LeftTuple = left;
            LeftTuple.ChildTuples.Add(this);
        }

        public Fact RightFact { get; private set; }
        public Tuple LeftTuple { get; private set; }
        public int Count { get; private set; }

        public T GetState<T>()
        {
            if (_state == null) _state = default(T);
            return (T) _state;
        }

        public void SetState(object value)
        {
            _state = value;
        }

        public IList<Tuple> ChildTuples
        {
            get { return _childTuples; }
        }

        public object[] GetFactObjects()
        {
            return this.Select(f => f.Object).ToArray();
        }

        public void Clear()
        {
            if (RightFact != null) RightFact.ChildTuples.Remove(this);
            RightFact = null;

            if (LeftTuple != null) LeftTuple.ChildTuples.Remove(this);
            LeftTuple = null;
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
                _index = 1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_index > _rootTuple._leftTuples.Count) return false;
                _currentTuple = (_index == _rootTuple._leftTuples.Count)
                                    ? _rootTuple
                                    : _rootTuple._leftTuples[_index];
                _index++;
                return true;
            }

            public void Reset()
            {
                _currentTuple = null;
                _index = 1;
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