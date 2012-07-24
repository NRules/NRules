using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class Declaration : IDeclaration, IEquatable<Declaration>
    {
        private readonly List<ICondition> _conditions = new List<ICondition>();

        public Declaration(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }

        public IList<ICondition> Conditions
        {
            get { return _conditions; }
        }

        public bool Equals(Declaration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Declaration)) return false;
            return Equals((Declaration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ Name.GetHashCode();
            }
        }
    }
}