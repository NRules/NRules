using System;

namespace NRules.Rule
{
    public class Declaration : IEquatable<Declaration>
    {
        internal Declaration(string name, Type type) : this(name, type, false)
        {
        }
        
        internal Declaration(string name, Type type, bool isLocal)
        {
            Name = name;
            Type = type;
            IsLocal = isLocal;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }

        public bool IsLocal { get; private set; }

        public PatternElement Target { get; internal set; }

        public bool Equals(Declaration other)
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
            return Equals((Declaration) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}