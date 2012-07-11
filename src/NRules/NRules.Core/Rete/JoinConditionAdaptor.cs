using System.Collections.Generic;
using System.Linq;

namespace NRules.Core.Rete
{
    internal class JoinConditionAdaptor
    {
        private readonly IJoinCondition _condition;
        private readonly int[] _factSelectionTable;

        public JoinConditionAdaptor(IJoinCondition condition, int[] factSelectionTable)
        {
            _condition = condition;
            _factSelectionTable = factSelectionTable;
        }

        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            //todo: optimize
            IEnumerable<Fact> facts =
                _factSelectionTable.Select(
                    idx => leftTuple.Reverse().ElementAtOrDefault(idx) ?? rightFact);

            return _condition.IsSatisfiedBy(facts.ToArray());
        }
    }
}