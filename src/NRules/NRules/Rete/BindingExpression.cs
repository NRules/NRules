using System;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IBindingExpression
    {
        LambdaExpression Expression { get; }
        object Invoke(IExecutionContext context, Tuple tuple);
    }

    internal class BindingExpression : IBindingExpression, IEquatable<BindingExpression>
    {
        private readonly FastDelegate<Func<object[], object>> _compiledExpression;
        private readonly IndexMap _factIndexMap;

        public BindingExpression(LambdaExpression expression, FastDelegate<Func<object[], object>> compiledExpression, IndexMap factIndexMap)
        {
            Expression = expression;
            _compiledExpression = compiledExpression;
            _factIndexMap = factIndexMap;
        }

        public LambdaExpression Expression { get; }
        
        public object Invoke(IExecutionContext context, Tuple tuple)
        {
            var args = new object[_compiledExpression.ArrayArgumentCount];
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                IndexMap.SetElementAt(args, _factIndexMap[index], fact.Object);
                index--;
            }

            try
            {
                return _compiledExpression.Delegate(args);
            }
            catch (Exception e)
            {
                throw new RuleExpressionEvaluationException("Failed to evaluate binding expression", Expression.ToString(), e);
            }
        }

        public bool Equals(BindingExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ExpressionComparer.AreEqual(Expression, other.Expression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingExpression)obj);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}
