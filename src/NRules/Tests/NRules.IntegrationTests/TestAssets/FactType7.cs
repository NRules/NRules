using System;

namespace NRules.IntegrationTests.TestAssets
{
    public class FactType7 : IEquatable<FactType7>
    {
        public long Id { get; set; }
        public int TestCount { get; set; }
        public string GroupingProperty { get; set; }
        public string GroupingProperty2 { get; set; }

        public FactType7 IncrementCount()
        {
            TestCount++;
            return this;
        }

        public bool Equals(FactType7 other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FactType7)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
