using System;

namespace NRules.Rete
{
    internal class Activation : IEquatable<Activation>
    {
        private readonly ICompiledRule _rule;
        private readonly int _priority;
        private readonly Tuple _tuple;
        private readonly IndexMap _tupleFactMap;

        public Activation(ICompiledRule rule, int priority, Tuple tuple, IndexMap tupleFactMap)
        {
            _rule = rule;
            _priority = priority;
            _tuple = tuple;
            _tupleFactMap = tupleFactMap;
        }

        public ICompiledRule Rule
        {
            get { return _rule; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public Tuple Tuple
        {
            get { return _tuple; }
        }

        public IndexMap TupleFactMap
        {
            get { return _tupleFactMap; }
        }

        public bool Equals(Activation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Rule, Rule) && Equals(other.Tuple, Tuple);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Activation)) return false;
            return Equals((Activation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Rule.GetHashCode()*397) ^ Tuple.GetHashCode();
            }
        }
    }
}