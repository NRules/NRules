using System.Collections.Generic;
using System.Linq;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal class JoinConditionAdaptor
    {
        private readonly ICondition _condition;
        private readonly int[] _factSelectionTable;

        public JoinConditionAdaptor(ICondition condition, int[] factSelectionTable)
        {
            _condition = condition;
            _factSelectionTable = factSelectionTable;
        }

        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            //todo: optimize
            IEnumerable<Fact> facts =
                _factSelectionTable.Select(
                    idx => leftTuple.ElementAtOrDefault(idx) ?? rightFact);

            var factObjects = facts.Select(f => f.Object).ToArray();
            return _condition.IsSatisfiedBy(factObjects);
        }
    }
}