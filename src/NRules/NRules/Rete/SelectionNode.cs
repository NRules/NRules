namespace NRules.Rete
{
    internal class SelectionNode : AlphaNode
    {
        private readonly IAlphaCondition _condition;

        public IAlphaCondition Condition { get { return _condition; } }

        public SelectionNode(IAlphaCondition condition)
        {
            _condition = condition;
        }

        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            return _condition.IsSatisfiedBy(context, fact);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitSelectionNode(context, this);
        }
    }
}