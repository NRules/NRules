using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    [DebuggerDisplay("Tuple ({Count})")]
    [DebuggerTypeProxy(typeof(TupleDebugView))]
    internal class Tuple
    {
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

        public virtual void Clear()
        {
            if (RightFact != null) RightFact.ChildTuples.Remove(this);
            RightFact = null;

            if (LeftTuple != null) LeftTuple.ChildTuples.Remove(this);
            LeftTuple = null;

            ChildTuples.Clear();
        }

        /// <summary>
        /// Facts contained in the tuple in reverse order (fast iteration over linked list).
        /// Reverse collection to get facts in their actual order.
        /// </summary>
        public IEnumerable<Fact> Facts
        {
            get
            {
                if (RightFact == null) yield break;

                yield return RightFact;
                foreach (var leftFact in LeftTuple.Facts)
                {
                    yield return leftFact;
                }
            }
        }

        internal class TupleDebugView
        {
            public readonly string Facts;

            public TupleDebugView(Tuple tuple)
            {
                Facts = string.Format("[{0}]", string.Join(" || ", tuple.Facts.Reverse().Select(f => f.Object).ToArray()));
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

        public override void Clear()
        {
            WrappedTuple.ChildTuples.Remove(this);

            ChildTuples.Clear();
        }

        public Tuple WrappedTuple { get; private set; }
    }
}