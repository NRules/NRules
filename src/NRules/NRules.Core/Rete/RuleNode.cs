namespace NRules.Core.Rete
{
    internal class RuleNode : ITupleSink
    {
        private readonly string _ruleHandle;
        private readonly int _rulePriority;

        public RuleNode(string ruleHandle, int rulePriority)
        {
            _ruleHandle = ruleHandle;
            _rulePriority = rulePriority;
        }

        public void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, _rulePriority, tuple);
            workingMemory.EventAggregator.Activate(activation);
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            //Do nothing
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, _rulePriority, tuple);
            workingMemory.EventAggregator.Deactivate(activation);
            tuple.Clear();
        }
    }
}