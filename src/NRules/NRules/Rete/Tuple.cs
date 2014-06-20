using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    [DebuggerDisplay("Tuple ({Count})")]
    [DebuggerTypeProxy(typeof(TupleDebugView))]
    internal class Tuple : IEnumerable<Fact>
    {
        private readonly List<Tuple> _leftTuples = new List<Tuple>();
        private readonly List<Tuple> _childTuples = new List<Tuple>();
        private object _state;

        public Tuple(INode node)
        {
            Node = node;
        }

        public Tuple(Tuple left, Fact right, INode node) : this(node)
        {
            Count = left.Count + 1;
            right.ChildTuples.Add(this);
            RightFact = right;
            _leftTuples.AddRange(left.LeftTuples);
            _leftTuples.Add(left);
            left.ChildTuples.Add(this);
            LeftTuple = left;
        }

        public INode Node { get; private set; }
        public virtual Fact RightFact { get; private set; }
        public virtual Tuple LeftTuple { get; private set; }
        public virtual int Count { get; private set; }

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

        public virtual IList<Tuple> LeftTuples
        {
            get { return _leftTuples; }
        }
        
        public virtual void Clear()
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
                if (_index > _rootTuple.LeftTuples.Count) return false;
                _currentTuple = (_index == _rootTuple.LeftTuples.Count)
                                    ? _rootTuple
                                    : _rootTuple.LeftTuples[_index];
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

        internal class TupleDebugView
        {
            public readonly string Facts;

            public TupleDebugView(Tuple tuple)
            {
                Facts = string.Format("[{0}]", string.Join(" || ", tuple.Select(f => f.Object).ToArray()));
            }
        }
    }

    internal class WrapperTuple : Tuple
    {
        public WrapperTuple(Tuple tuple, INode node) : base(node)
        {
            WrappedTuple = tuple;
            WrappedTuple.ChildTuples.Add(this);
        }

        public override Fact RightFact
        {
            get { return WrappedTuple.RightFact; }
        }

        public override Tuple LeftTuple
        {
            get { return WrappedTuple.LeftTuple; }
        }

        public override int Count
        {
            get { return WrappedTuple.Count; }
        }

        public override IList<Tuple> LeftTuples
        {
            get { return WrappedTuple.LeftTuples; }
        }

        public override void Clear()
        {
            WrappedTuple.ChildTuples.Remove(this);

            ChildTuples.Clear();
        }

        public Tuple WrappedTuple { get; private set; }
    }
}