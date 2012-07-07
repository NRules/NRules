namespace NRules.Core.Rete
{
    internal class SelectionNode : AlphaNode
    {
        public ICondition Condition { get; private set; }

        public SelectionNode(ICondition condition)
        {
            Condition = condition;
        }

        public bool IsSatisfiedBy(Fact fact)
        {
            return Condition.IsSatisfiedBy(fact);
        }

        public override void PropagateAssert(Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateAssert(fact);
                }
            }
        }
    }
}