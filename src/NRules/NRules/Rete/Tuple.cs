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

        private Dictionary<INode, object> _stateMap;

        public Tuple()
        {
            Id = GetHashCode();
            GroupId = NoGroup;
            Count = 0;
            Level = 0;
        }

        public Tuple(Tuple left, Fact right) : this()
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
            if (_stateMap == null) _stateMap = new Dictionary<INode, object>();
            _stateMap[node] = value;
        }

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
        public IEnumerable<Fact> Facts
        {
            get
            {
                var tuple = this;
                while (tuple != null)
                {
                    if (tuple.RightFact != null)
                        yield return tuple.RightFact;

                    tuple = tuple.LeftTuple;
                }
            }
        }

        /// <summary>
        /// Facts contained in the tuple in correct order.
        /// </summary>
        /// <remarks>This method has to reverse the linked list and is slow.</remarks>
        public IEnumerable<Fact> OrderedFacts => Facts.Reverse();

        IEnumerable<IFact> ITuple.Facts => Facts;

        internal class TupleDebugView
        {
            public readonly string Facts;

            public TupleDebugView(Tuple tuple)
            {
                var facts = string.Join(" || ", tuple.Facts.Reverse().Select(f => f.Object).ToArray());
                Facts = $"[{facts}]";
            }
        }
    }
}