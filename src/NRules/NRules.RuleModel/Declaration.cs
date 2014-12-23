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
        private readonly string _fullName;
        private readonly string _name;
        private readonly string _scopeName;
        private readonly Type _type;

        internal Declaration(Type type, string name, string scopeName)
        {
            _type = type;
            _name = name;
            _scopeName = scopeName;
            _fullName = (scopeName == null) ? Name : ScopeName + SymbolTable.ScopeSeparator + Name;
        }

        /// <summary>
        /// Symbol name qualified with the scope name.
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
        }

        /// <summary>
        /// Symbol name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Name of the enclosing scope.
        /// </summary>
        public string ScopeName
        {
            get { return _scopeName; }
        }

        /// <summary>
        /// Symbol type.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Rule element that this declaration is referencing.
        /// </summary>
        public PatternElement Target { get; internal set; }

        public bool Equals(Declaration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_fullName, other._fullName);
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
            return _fullName.GetHashCode();
        }
    }
}