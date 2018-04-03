using System;
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
        
        public T GetState<T>(INode node)
        {
            if (_stateMap != null && _stateMap.TryGetValue(node, out var value))
            {
                return (T) value;
            }
            return default(T);
        }
        
        public T GetStateOrThrow<T>(INode node)
        {
            if (_stateMap != null && _stateMap.TryGetValue(node, out var value))
            {
                return (T) value;
            }
            throw new ArgumentException($"Tuple state not found. NodeType={node.GetType()}, StateType={typeof(T)}");
        }

        public T RemoveState<T>(INode node)
        {
            if (_stateMap != null && _stateMap.TryGetValue(node, out var value))
            {
                var state = (T)value;
                _stateMap.Remove(node);
                return state;
            }
            return default(T);
        }

        public T RemoveStateOrThrow<T>(INode node)
        {
            if (_stateMap != null && _stateMap.TryGetValue(node, out var value))
            {
                var state = (T)value;
                _stateMap.Remove(node);
                return state;
            }
            throw new ArgumentException($"Tuple state not found. NodeType={node.GetType()}, StateType={typeof(T)}");
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