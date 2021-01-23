using System;
using System.Collections.Generic;
using NRules.Collections;

namespace NRules.Rete
{
    internal interface IBetaMemory
    {
        IEnumerable<Tuple> Tuples { get; }
        void Add(Tuple tuple);
        void Remove(Tuple tuple);
        Tuple FindTuple(Tuple leftTuple, Fact rightFact);
    }

    internal class BetaMemory : IBetaMemory
    {
        private readonly OrderedHashSet<Tuple> _tuples = new OrderedHashSet<Tuple>();
        private readonly Dictionary<Key, Tuple> _parentToChildMap = new Dictionary<Key, Tuple>(); 

        public IEnumerable<Tuple> Tuples => _tuples;

        public void Add(Tuple tuple)
        {
            _tuples.Add(tuple);
            AddMapping(tuple);
        }

        public void Remove(Tuple tuple)
        {
            _tuples.Remove(tuple);
            RemoveMapping(tuple);
        }

        public Tuple FindTuple(Tuple leftTuple, Fact rightFact)
        {
            var key = new Key(leftTuple, rightFact);
            _parentToChildMap.TryGetValue(key, out var childTuple);
            return childTuple;
        }

        private void AddMapping(Tuple tuple)
        {
            if (tuple.LeftTuple == null) return;
            var key = new Key(tuple.LeftTuple, tuple.RightFact);
            _parentToChildMap[key] = tuple;
        }

        private void RemoveMapping(Tuple tuple)
        {
            if (tuple.LeftTuple == null) return;
            var key = new Key(tuple.LeftTuple, tuple.RightFact);
            _parentToChildMap.Remove(key);
        }

        private readonly struct Key : IEquatable<Key>
        {
            private readonly Tuple _tuple;
            private readonly Fact _fact;

            public Key(Tuple tuple, Fact fact)
            {
                _tuple = tuple;
                _fact = fact;
            }

            public bool Equals(Key other)
            {
                return ReferenceEquals(_tuple, other._tuple) && ReferenceEquals(_fact, other._fact);
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_tuple.GetHashCode() * 397) ^ (_fact != null ? _fact.GetHashCode() : 0);
                }
            }
        }
    }
}