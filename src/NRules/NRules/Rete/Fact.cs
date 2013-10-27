using System;
using System.Collections.Generic;

namespace NRules.Rete
{
    internal class Fact : IEquatable<Fact>
    {
        public Fact(object @object)
        {
            Object = @object;
            FactType = @object.GetType();
            ChildTuples = new List<Tuple>();
        }

        public Type FactType { get; private set; }
        public object Object { get; private set; }
        public IList<Tuple> ChildTuples { get; private set; }

        public bool Equals(Fact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Object.Equals(other.Object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Fact) obj);
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }
    }
}