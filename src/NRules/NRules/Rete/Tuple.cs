using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    [DebuggerDisplay("Tuple ({Count})")]
    [DebuggerTypeProxy(typeof(TupleDebugView))]
    internal class Tuple
    {
        private object _state;

        public Tuple()
        {
            Count = 0;
        }

        public Tuple(Tuple left, Fact right)
        {
            RightFact = right;
            LeftTuple = left;
            Count = left.Count;
            if (right != null) Count++;
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

        public void Clear()
        {
            RightFact = null;
            LeftTuple = null;
        }

        /// <summary>
        /// Facts contained in the tuple in reverse order (fast iteration over linked list).
        /// Reverse collection to get facts in their actual order.
        /// </summary>
        public IEnumerable<Fact> Facts
        {
            get
            {
                if (RightFact != null) yield return RightFact;
                if (LeftTuple == null) yield break;
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
}