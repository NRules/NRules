namespace NRules.Core.Rete
{
    internal class RuleNode : ITupleSink
    {
        private readonly string _ruleHandle;

        public RuleNode(string ruleHandle)
        {
            _ruleHandle = ruleHandle;
        }

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, tuple);
            workingMemory.EventAggregator.Activate(activation);
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, tuple);
            workingMemory.EventAggregator.Deactivate(activation);
            tuple.Clear();
        }
    }
}