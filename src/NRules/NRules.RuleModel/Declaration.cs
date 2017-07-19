using System;
using System.Diagnostics;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern declaration.
    /// </summary>
    [DebuggerDisplay("{FullName}: {Type}")]
    public class Declaration : IEquatable<Declaration>
    {
        internal Declaration(Type type, string name, string scopeName)
        {
            Type = type;
            Name = name;
            FullName = scopeName == null ? Name : scopeName + SymbolTable.ScopeSeparator + Name;
        }

        /// <summary>
        /// Symbol name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Symbol name qualified with full scope.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Symbol type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Rule element that this declaration is referencing.
        /// </summary>
        public RuleElement Target { get; internal set; }

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