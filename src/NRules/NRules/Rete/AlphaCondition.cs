using System;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IAlphaCondition
    {
        bool IsSatisfiedBy(IExecutionContext context, Fact fact);
    }

    internal class AlphaCondition : IAlphaCondition, IEquatable<AlphaCondition>
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object, bool>> _compiledExpression;

        public AlphaCondition(LambdaExpression expression)
        {
            _expression = expression;
            _compiledExpression = FastDelegate.AlphaCondition(expression);
        }

        public bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            try
            {
                return _compiledExpression.Delegate(fact.Object);
            }
            catch (Exception e)
            {
                bool isHandled = false;
                context.EventAggregator.RaiseConditionFailed(context.Session, e, _expression, null, fact, ref isHandled);
                if (!isHandled)
                {
                    throw new RuleConditionEvaluationException("Failed to evaluate condition", _expression.ToString(), e);
                }
                return false;
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