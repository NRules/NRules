namespace NRules.Core.Rete
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

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            _ruleNode.Activate(workingMemory, tuple);
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            _ruleNode.Deactivate(workingMemory, tuple);
            tuple.Clear();
        }

        public void Attach(IRuleNode ruleNode)
        {
            _ruleNode = ruleNode;
        }
    }
}