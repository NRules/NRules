using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern declaration.
    /// </summary>
    public class Declaration : IEquatable<Declaration>
    {
        internal Declaration(Type type, string name, string scopeName)
        {
            Type = type;
            Name = name;
            ScopeName = scopeName;
            FullName = (scopeName == null) ? Name : ScopeName + SymbolTable.ScopeSeparator + Name;
        }

        /// <summary>
        /// Symbol name qualified with the scope name.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Symbol name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Name of the enclosing scope.
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        /// Symbol type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Rule element that this declaration is referencing.
        /// </summary>
        public PatternElement Target { get; internal set; }

        public bool Equals(Declaration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(FullName, other.FullName);
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
            return FullName.GetHashCode();
        }
    }
}