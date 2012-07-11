using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class Fact : IEquatable<Fact>
    {
        public Fact(object @object)
        {
            Object = @object;
            FactType = Object.GetType();
            ChildTuples = new List<Tuple>();
        }

        public Type FactType { get; private set; }
        public object Object { get; private set; }
        public IList<Tuple> ChildTuples { get; private set; }

        public bool Equals(Fact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FactType, FactType) && Equals(other.Object, Object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Fact)) return false;
            return Equals((Fact) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FactType.GetHashCode()*397) ^ Object.GetHashCode();
            }
        }
    }
}