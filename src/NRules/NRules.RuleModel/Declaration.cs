using System;
using System.Diagnostics;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern declaration.
    /// </summary>
    [DebuggerDisplay("{_fullName}: {_type}")]
    public class Declaration : IEquatable<Declaration>
    {
        private readonly string _fullName;
        private readonly string _name;
        private readonly Type _type;

        internal Declaration(Type type, string name, string scopeName)
        {
            _type = type;
            _name = name;
            _fullName = (scopeName == null) ? _name : scopeName + SymbolTable.ScopeSeparator + _name;
        }

        /// <summary>
        /// Symbol name.
        /// </summary>
        public string Name
        {
            get { return _name; }
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
        public RuleElement Target { get; internal set; }

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