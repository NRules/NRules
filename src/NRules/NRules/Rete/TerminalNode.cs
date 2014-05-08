namespace NRules.Rete
{
    internal interface ITerminalNode
    {
        void Attach(IRuleNode ruleNode);
    }

    internal class TerminalNode : ITerminalNode, ITupleSink
    {
        public IRuleNode RuleNode { get; private set; }

        public TerminalNode(ITupleSource source)
        {
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            RuleNode.Activate(context, tuple);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            RuleNode.Deactivate(context, tuple);
        }

        public void Attach(IRuleNode ruleNode)
        {
            RuleNode = ruleNode;
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitTerminalNode(context, this);
        }
    }
}