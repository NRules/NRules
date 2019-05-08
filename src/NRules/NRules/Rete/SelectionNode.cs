namespace NRules.Rete
{
    internal class SelectionNode : AlphaNode
    {
        public IAlphaCondition Condition { get; }

        public SelectionNode(IAlphaCondition condition)
        {
            Condition = condition;
        }

        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            return Condition.IsSatisfiedBy(context, NodeInfo, fact);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitSelectionNode(context, this);
        }
    }
}