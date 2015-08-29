using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    [DebuggerDisplay("Tuple ({Count})")]
    [DebuggerTypeProxy(typeof(TupleDebugView))]
    internal class Tuple
    {
        private readonly Dictionary<INode, object> _stateMap = new Dictionary<INode, object>();
        private readonly Fact _rightFact;
        private readonly Tuple _leftTuple;
        private readonly int _count;

        public Tuple()
        {
            _count = 0;
        }

        public Tuple(Tuple left, Fact right)
        {
            _rightFact = right;
            _leftTuple = left;
            _count = left.Count;
            if (right != null) _count++;
        }

        public Fact RightFact { get { return _rightFact; } }
        public Tuple LeftTuple { get { return _leftTuple; } }
        public int Count { get { return _count; } }

        public T GetState<T>(INode node)
        {
            object value;
            if (_stateMap != null && _stateMap.TryGetValue(node, out value))
            {
                return (T) value;
            }
            return default(T);
        }

        public void SetState(INode node, object value)
        {
            _stateMap[node] = value;
        }

        /// <summary>
        /// Facts contained in the tuple in reverse order (fast iteration over linked list).
        /// Reverse collection to get facts in their actual order.
        /// </summary>
        public IEnumerable<Fact> Facts
        {
            get
            {
                if (_rightFact != null) yield return _rightFact;
                if (_leftTuple == null) yield break;
                foreach (var leftFact in _leftTuple.Facts)
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