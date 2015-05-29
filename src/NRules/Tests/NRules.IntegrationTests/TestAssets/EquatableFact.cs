using System;

namespace NRules.IntegrationTests.TestAssets
{
    public class EquatableFact : IEquatable<EquatableFact>
    {
        public EquatableFact(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string TestProperty { get; set; }

        public bool Equals(EquatableFact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EquatableFact)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
