using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Core.Rete
{
    internal class BetaCondition : Condition
    {
        public int[] FactSelectionTable { get; set; }

        public BetaCondition(LambdaExpression expression) : base(expression)
        {
        }

        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            //todo: optimize
            IEnumerable<Fact> facts =
                FactSelectionTable.Select(
                    idx => leftTuple.ElementAtOrDefault(idx) ?? rightFact);

            var factObjects = facts.Select(f => f.Object).ToArray();
            return IsSatisfiedBy(factObjects);
        }
    }
}