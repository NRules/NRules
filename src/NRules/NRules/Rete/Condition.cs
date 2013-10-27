using System;
using System.Linq.Expressions;

namespace NRules.Rete
{
    internal abstract class Condition : IEquatable<Condition>
    {
        private readonly string _key;
        private readonly Delegate _compiledExpression;

        protected Condition(LambdaExpression expression)
        {
            _key = expression.ToString();
            _compiledExpression = expression.Compile();
        }

        protected bool IsSatisfiedBy(params object[] factObjects)
        {
            return (bool) _compiledExpression.DynamicInvoke(factObjects);
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