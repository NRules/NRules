namespace NRules.Rete
{
    internal class RootNode : AlphaNode
    {
        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            return true;
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRootNode(context, this);
        }
    }
}