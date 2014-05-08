using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    internal class SelectionNode : AlphaNode
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<AlphaCondition> Conditions { get; private set; }

        public SelectionNode(AlphaCondition condition)
        {
            Conditions = new List<AlphaCondition>();
            Conditions.Add(condition);
        }

        public override bool IsSatisfiedBy(Fact fact)
        {
            return Conditions.All(c => c.IsSatisfiedBy(fact));
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitSelectionNode(context, this);
        }
    }
}