namespace NRules.Rete
{
    internal interface ITerminalNode
    {
        void Attach(IRuleNode ruleNode);
    }

    internal class TerminalNode : ITerminalNode, ITupleSink
    {
        private IRuleNode _ruleNode;

        public TerminalNode(ITupleSource source)
        {
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            _ruleNode.Activate(context, tuple);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            _ruleNode.Deactivate(context, tuple);
        }

        public void Attach(IRuleNode ruleNode)
        {
            _ruleNode = ruleNode;
        }
    }
}