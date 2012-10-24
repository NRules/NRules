using System.Collections.Generic;
using System.Linq;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal class SelectionNode : AlphaNode
    {
        public IList<ICondition> Conditions { get; private set; }

        public SelectionNode(ICondition condition)
        {
            Conditions = new List<ICondition>();
            Conditions.Add(condition);
        }

        public override bool IsSatisfiedBy(Fact fact)
        {
            return Conditions.All(c => c.IsSatisfiedBy(fact.Object));
        }
    }
}