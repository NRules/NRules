using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete
{
    [DebuggerDisplay("Tuple ({Count})")]
    [DebuggerTypeProxy(typeof(TupleDebugView))]
    internal sealed class Tuple : ITuple
    {
        private const long NoGroup = 0;

        public Tuple(long id)
        {
            Id = id;
            GroupId = NoGroup;
            Count = 0;
            Level = 0;
        }

        public Tuple(long id, Tuple left, Fact right) : this(id)
        {
            RightFact = right;
            LeftTuple = left;
            Count = left.Count;
            if (right != null) Count++;
            Level = left.Level + 1;
        }

        public Fact RightFact { get; }
        public Tuple LeftTuple { get; }
        public int Count { get; }
        public long Id { get; }
        public int Level { get; }

        public long GroupId { get; set; }
        
        public long GetGroupId(int level)
        {
            if (level == Level - 1) return GroupId;
            long groupId = LeftTuple?.GetGroupId(level) ?? NoGroup;
            return groupId;
        }

        /// <summary>
        /// Facts contained in the tuple in reverse order (fast iteration over linked list).
        /// Reverse collection to get facts in their actual order.
        /// </summary>
        public IEnumerable<IFact> Facts => new Enumerable(this);

        public Enumerator GetEnumerator() => new Enumerator(this);

        internal class TupleDebugView
        {
            public readonly string Facts;

            public TupleDebugView(Tuple tuple)
            {
                var facts = string.Join(" || ", tuple.Facts.Reverse().Select(f => f.Value).ToArray());
                Facts = $"[{facts}]";
            }
        }

        internal class Enumerable : IEnumerable<Fact>, IEnumerator<Fact>
        {
            private readonly Tuple _tuple;
            private Enumerator _enumerator;

            public Enumerable(Tuple tuple)
            {
                _tuple = tuple;
                _enumerator = new Enumerator(tuple);
            }

            public IEnumerator<Fact> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Dispose() {}
            public bool MoveNext() => _enumerator.MoveNext();
            public void Reset() => _enumerator = new Enumerator(_tuple);
            public Fact Current => _enumerator.Current;
            object IEnumerator.Current => Current;
        }

        internal struct Enumerator
        {
            private Tuple _tuple;

            public Enumerator(Tuple tuple)
            {
                _tuple = tuple;
                Current = null;
            }

            public bool MoveNext()
            {
                do
                {
                    Current = _tuple.RightFact;
                    _tuple = _tuple.LeftTuple;
                } while (Current == null && _tuple != null);
                return Current != null;
            }

            public Fact Current { get; private set; }
        }
    }
}