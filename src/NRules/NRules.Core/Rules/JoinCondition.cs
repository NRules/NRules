using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Core.Rete;

namespace NRules.Core.Rules
{
    internal class JoinCondition<T1, T2> : IJoinCondition
    {
        private readonly Expression<Func<T1, T2, bool>> _expression;
        private readonly Func<T1, T2, bool> _compiledExpression;
        private readonly IList<Type> _factTypes = new List<Type>();

        public string Key { get; private set; }

        public IEnumerable<Type> FactTypes
        {
            get { return _factTypes; }
        }

        public JoinCondition(Expression<Func<T1, T2, bool>> expression)
        {
            _expression = expression;
            _compiledExpression = _expression.Compile();
            _factTypes.Add(typeof (T1));
            _factTypes.Add(typeof (T2));
            Key = _expression.ToString();
        }

        public bool IsSatisfiedBy(params Fact[] facts)
        {
            try
            {
                bool result = _compiledExpression.Invoke((T1) facts[0].Object, (T2) facts[1].Object);
                return result;
            }
            catch (InvalidCastException e)
            {
                throw new InvalidOperationException("Fact type does not match condition type", e);
            }
        }
    }
}