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

        public override void PropagateUpdate(Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateUpdate(fact);
                }
            }
            else
            {
                ForceRetract(fact);
            }
        }

        public override void PropagateRetract(Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateRetract(fact);
                }
            }
        }

        public override void ForceRetract(Fact fact)
        {
            foreach (var childNode in ChildNodes)
            {
                childNode.ForceRetract(fact);
            }
        }
    }
}