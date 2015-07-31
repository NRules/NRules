using System;

namespace NRules.IntegrationTests.TestAssets
{
    public class FactType1Projection : IEquatable<FactType1Projection>
    {
        public FactType1Projection(FactType1 fact)
        {
            Value = fact.TestProperty;
        }

        public string Value { get; private set; }

        public bool Equals(FactType1Projection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FactType1Projection)obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}