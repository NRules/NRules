using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern declaration.
    /// </summary>
    public class Declaration : IEquatable<Declaration>
    {
        internal Declaration(Type type, string name, bool isLocal)
        {
            Name = name;
            Type = type;
            IsLocal = isLocal;
        }

        /// <summary>
        /// Symbol name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Symbol type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Indicates whether the symbol is local or exposed in an outer scope.
        /// </summary>
        public bool IsLocal { get; private set; }

        /// <summary>
        /// Rule element this declaration is referencing.
        /// </summary>
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