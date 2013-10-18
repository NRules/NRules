using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Core.Rete
{
    internal abstract class Condition : IEquatable<Condition>
    {
        private readonly string _key;
        private readonly Delegate _compiledExpression;

        protected Condition(LambdaExpression expression)
        {
            _key = expression.ToString();
            FactTypes = expression.Parameters.Select(p => p.Type).ToList();
            _compiledExpression = expression.Compile();
        }

        public IEnumerable<Type> FactTypes { get; private set; }

        protected bool IsSatisfiedBy(params object[] factObjects)
        {
            return (bool)_compiledExpression.DynamicInvoke(factObjects);
        }

        public bool Equals(Condition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_key, other._key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Condition) obj);
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }
    }
}
