using System;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IAlphaCondition
    {
        bool IsSatisfiedBy(IExecutionContext context, NodeDebugInfo nodeInfo, Fact fact);
    }

    internal sealed class AlphaCondition : IAlphaCondition, IEquatable<AlphaCondition>
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object, bool>> _compiledExpression;

        public AlphaCondition(LambdaExpression expression, FastDelegate<Func<object, bool>> compiledExpression)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public bool IsSatisfiedBy(IExecutionContext context, NodeDebugInfo nodeInfo, Fact fact)
        {
            var factValue = fact.Object;
            Exception exception = null;
            bool result = false;
            try
            {
                result = _compiledExpression.Delegate(factValue);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, factValue, null, fact, nodeInfo, ref isHandled);
                if (!isHandled)
                {
                    throw new RuleLhsExpressionEvaluationException("Failed to evaluate condition", _expression.ToString(), e);
                }
                return false;
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, factValue, result, null, fact, nodeInfo);
            }
        }

        public bool Equals(AlphaCondition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ExpressionComparer.AreEqual(_expression, other._expression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AlphaCondition)obj);
        }

        public override int GetHashCode()
        {
            return _expression.GetHashCode();
        }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}