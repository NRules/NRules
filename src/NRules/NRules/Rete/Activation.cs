using System;

namespace NRules.Rete
{
    internal class Activation : IEquatable<Activation>
    {
        public Activation(ICompiledRule rule, Tuple tuple)
        {
            Rule = rule;
            Tuple = tuple;
        }

        public ICompiledRule Rule { get; private set; }
        public Tuple Tuple { get; private set; }

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