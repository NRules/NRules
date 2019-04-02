using System;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IBindingExpression
    {
        LambdaExpression Expression { get; }
        object Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple);
    }

    internal sealed class BindingExpression : IBindingExpression, IEquatable<BindingExpression>
    {
        private readonly FastDelegate<Func<object[], object>> _compiledExpression;
        private readonly IndexMap _factMap;

        public BindingExpression(LambdaExpression expression, FastDelegate<Func<object[], object>> compiledExpression, IndexMap factMap)
        {
            Expression = expression;
            _compiledExpression = compiledExpression;
            _factMap = factMap;
        }

        public LambdaExpression Expression { get; }
        
        public object Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple)
        {
            var args = new object[_compiledExpression.ArrayArgumentCount];
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                IndexMap.SetElementAt(args, _factMap[index], fact.Object);
                index--;
            }

            Exception exception = null;
            object result = null;
            try
            {
                result = _compiledExpression.Delegate(args);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, Expression, args, tuple, null, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, Expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, Expression, args, result, tuple, null, nodeInfo);
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
