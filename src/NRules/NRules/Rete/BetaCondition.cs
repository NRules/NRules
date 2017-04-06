using System;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IBetaCondition
    {
        bool IsSatisfiedBy(IExecutionContext context, Tuple leftTuple, Fact rightFact);
    }

    internal class BetaCondition : IBetaCondition, IEquatable<BetaCondition>
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factIndexMap;
        private readonly FastDelegate<Func<object[], bool>> _compiledExpression;

        public BetaCondition(LambdaExpression expression, IndexMap factIndexMap)
        {
            _expression = expression;
            _factIndexMap = factIndexMap;
            _compiledExpression = FastDelegate.BetaCondition(expression);
        }

        public bool IsSatisfiedBy(IExecutionContext context, Tuple leftTuple, Fact rightFact)
        {
            var args = new object[_compiledExpression.ArrayArgumentCount];
            int index = leftTuple.Count - 1;
            foreach (var fact in leftTuple.Facts)
            {
                IndexMap.SetElementAt(args, _factIndexMap[index], fact.Object);
                index--;
            }
            IndexMap.SetElementAt(args, _factIndexMap[leftTuple.Count], rightFact.Object);

            try
            {
                return _compiledExpression.Delegate(args);
            }
            catch (Exception e)
            {
                bool isHandled = false;
                context.EventAggregator.RaiseConditionFailed(context.Session, e, _expression, leftTuple, rightFact, ref isHandled);
                if (!isHandled)
                {
                    throw new RuleConditionEvaluationException("Failed to evaluate condition", _expression.ToString(), e);
                }
                return false;
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