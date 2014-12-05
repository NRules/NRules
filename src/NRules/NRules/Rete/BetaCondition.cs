using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IBetaCondition
    {
        bool IsSatisfiedBy(IExecutionContext context, Tuple leftTuple, Fact rightFact);
    }

    [DebuggerDisplay("{_expression.ToString()}")]
    internal class BetaCondition : IBetaCondition, IEquatable<BetaCondition>
    {
        private readonly LambdaExpression _expression;
        private readonly TupleMask _tupleMask;
        private readonly Func<object[], bool> _compiledExpression;

        public BetaCondition(LambdaExpression expression, TupleMask tupleMask)
        {
            _expression = expression;
            _tupleMask = tupleMask;
            _compiledExpression = FastDelegate.Create<Func<object[], bool>>(expression);
        }

        public bool IsSatisfiedBy(IExecutionContext context, Tuple leftTuple, Fact rightFact)
        {
            var args = new object[leftTuple.Count + 1];
            int index = leftTuple.Count - 1;
            foreach (var fact in leftTuple.Facts)
            {
                _tupleMask.SetAtIndex(ref args, index, fact.Object);
                index--;
            }
            _tupleMask.SetAtIndex(ref args, leftTuple.Count, rightFact.Object);

            try
            {
                return _compiledExpression(args);
            }
            catch (Exception e)
            {
                context.EventAggregator.RaiseConditionFailed(context.Session, e, _expression, leftTuple, rightFact);
                throw new RuleConditionEvaluationException("Failed to evaluate condition", _expression.ToString(), e);
            }
        }

        public bool Equals(BetaCondition other)
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
            return Equals((BetaCondition)obj);
        }

        public override int GetHashCode()
        {
            return (_expression != null ? _expression.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}