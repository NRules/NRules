using System;

namespace NRules.Samples.MissManners.Domain
{
    public class Hobby : IEquatable<Hobby>
    {
        public string Name { get; }

        public Hobby(string name)
        {
            Name = name;
        }

        public bool Equals(Hobby other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Hobby) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Hobby left, Hobby right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Hobby left, Hobby right)
        {
            return !Equals(left, right);
        }
    }
}