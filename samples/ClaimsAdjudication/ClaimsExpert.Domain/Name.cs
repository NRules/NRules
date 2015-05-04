using System;

namespace NRules.Samples.ClaimsExpert.Domain
{
    public struct Name : IEquatable<Name>
    {
        public static readonly Name Empty = new Name();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public bool IsEmpty
        {
            get { return Equals(Empty); }
        }

        public bool Equals(Name other)
        {
            return string.Equals(FirstName, other.FirstName) &&
                   string.Equals(LastName, other.LastName) &&
                   string.Equals(MiddleName, other.MiddleName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Name && Equals((Name) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MiddleName != null ? MiddleName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}