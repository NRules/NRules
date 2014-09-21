using System.Linq.Expressions;

namespace NRules.Rete
{
    internal interface IBetaCondition
    {
        bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact);
    }

    internal class BetaCondition : Condition, IBetaCondition
    {
        private readonly TupleMask _tupleMask;

        public BetaCondition(LambdaExpression expression, TupleMask tupleMask)
            : base(expression)
        {
            _tupleMask = tupleMask;
        }

        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            var args = new object[leftTuple.Count + 1];
            int index = leftTuple.Count - 1;
            foreach (var fact in leftTuple.Facts)
            {
                _tupleMask.SetAtIndex(ref args, index, fact.Object);
                index--;
            }
            _tupleMask.SetAtIndex(ref args, leftTuple.Count, rightFact.Object);

            return IsSatisfiedBy(args);
        }
    }
}