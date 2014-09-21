using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    [DebuggerDisplay("{_expressionString}")]
    internal abstract class Condition : IEquatable<Condition>
    {
        private readonly string _expressionString;
        private readonly Func<object[], bool> _compiledExpression;

        protected Condition(LambdaExpression expression)
        {
            _expressionString = expression.ToString();
            _compiledExpression = FastDelegate.Create<Func<object[], bool>>(expression);
        }

        public string ExpressionString { get { return _expressionString; } }

        protected bool IsSatisfiedBy(params object[] factObjects)
        {
            return _compiledExpression(factObjects);
        }

        public bool Equals(Condition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_expressionString, other._expressionString);
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
            return _expressionString.GetHashCode();
        }

        public override string ToString()
        {
            return ExpressionString;
        }
    }
}