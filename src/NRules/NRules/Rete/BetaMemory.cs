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
        private static readonly Fact NullFact = new Fact();

        private readonly OrderedHashSet<Tuple> _tuples = new OrderedHashSet<Tuple>();
        private readonly Dictionary<Tuple, Dictionary<Fact, Tuple>> _parentToChildMap = new Dictionary<Tuple, Dictionary<Fact, Tuple>>(); 

        public IEnumerable<Tuple> Tuples
        {
            get { return _tuples; }
        }

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
            Dictionary<Fact, Tuple> subMap;
            if (_parentToChildMap.TryGetValue(leftTuple, out subMap))
            {
                Tuple childTuple;
                subMap.TryGetValue(rightFact ?? NullFact, out childTuple);
                return childTuple;
            }
            return null;
        }

        private void AddMapping(Tuple tuple)
        {
            if (tuple.LeftTuple == null) return;
            Dictionary<Fact, Tuple> subMap;
            if (!_parentToChildMap.TryGetValue(tuple.LeftTuple, out subMap))
            {
                subMap = new Dictionary<Fact, Tuple>();
                _parentToChildMap[tuple.LeftTuple] = subMap;
            }
            subMap[tuple.RightFact ?? NullFact] = tuple;
        }

        private void RemoveMapping(Tuple tuple)
        {
            if (tuple.LeftTuple == null) return;
            Dictionary<Fact, Tuple> subMap;
            if (_parentToChildMap.TryGetValue(tuple.LeftTuple, out subMap))
            {
                subMap.Remove(tuple.RightFact ?? NullFact);
                if (subMap.Count == 0) _parentToChildMap.Remove(tuple.LeftTuple);
            }
        }
    }
}