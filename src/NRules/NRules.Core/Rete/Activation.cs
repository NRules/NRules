using System;

namespace NRules.Core.Rete
{
    internal class Activation : IEquatable<Activation>
    {
        public Activation(string ruleHandle, Tuple tuple)
        {
            RuleHandle = ruleHandle;
            Tuple = tuple;
        }

        public string RuleHandle { get; private set; }
        public Tuple Tuple { get; private set; }

        public bool Equals(Activation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.RuleHandle, RuleHandle) && Equals(other.Tuple, Tuple);
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
                return (RuleHandle.GetHashCode()*397) ^ Tuple.GetHashCode();
            }
        }
    }
}