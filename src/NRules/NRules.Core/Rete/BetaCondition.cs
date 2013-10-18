using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Core.Rete
{
    internal class BetaCondition : Condition
    {
        private readonly int[] _factSelectionTable;

        public BetaCondition(LambdaExpression expression, int[] factSelectionTable)
            : base(expression)
        {
            _factSelectionTable = factSelectionTable;
        }

        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            //todo: optimize
            IEnumerable<Fact> facts =
                _factSelectionTable.Select(
                    idx => leftTuple.ElementAtOrDefault(idx) ?? rightFact);

            var factObjects = facts.Select(f => f.Object).ToArray();
            return IsSatisfiedBy(factObjects);
        }
    }
}