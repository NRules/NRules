using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Rete
{
    internal interface IBetaCondition
    {
        bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact);
    }

    internal class BetaCondition : Condition, IBetaCondition
    {
        private readonly int[] _tupleMask;

        public BetaCondition(LambdaExpression expression, int[] tupleMask)
            : base(expression)
        {
            _tupleMask = tupleMask;
        }

        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            //todo: optimize
            IEnumerable<Fact> facts =
                _tupleMask.Select(
                    idx => leftTuple.ElementAtOrDefault(idx) ?? rightFact);

            object[] factObjects = facts.Select(f => f.Object).ToArray();
            return IsSatisfiedBy(factObjects);
        }
    }
}