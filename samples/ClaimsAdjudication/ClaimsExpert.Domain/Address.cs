using System;

namespace NRules.Samples.ClaimsExpert.Domain
{
    public struct Address : IEquatable<Address>
    {
        public static readonly Address Empty = new Address();

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool IsEmpty { get { return Equals(Empty); } }

        public bool Equals(Address other)
        {
            return string.Equals(Line1, other.Line1) &&
                   string.Equals(Line2, other.Line2) &&
                   string.Equals(City, other.City) &&
                   string.Equals(State, other.State) &&
                   string.Equals(Zip, other.Zip);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Address && Equals((Address) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Line1 != null ? Line1.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Line2 != null ? Line2.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Zip != null ? Zip.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}