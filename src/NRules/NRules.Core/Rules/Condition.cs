using System;
using System.Linq.Expressions;
using NRules.Core.Rete;

namespace NRules.Core.Rules
{
    internal class Condition<T> : ICondition
    {
        private readonly Expression<Func<T, bool>> _expression;
        private readonly Func<T, bool> _compiledExpression;

        public string Key { get; private set; }
        public Type FactType { get; private set; }

        public Condition(Expression<Func<T, bool>> expression)
        {
            _expression = expression;
            _compiledExpression = _expression.Compile();
            FactType = typeof (T);
            Key = _expression.ToString();
        }

        public bool IsSatisfiedBy(Fact fact)
        {
            try
            {
                bool result = _compiledExpression.Invoke((T) fact.Object);
                return result;
            }
            catch (InvalidCastException e)
            {
                throw new InvalidOperationException("Fact type does not match condition type", e);
            }
        }
    }
}