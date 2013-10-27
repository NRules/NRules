namespace NRules.Rete
{
    internal interface IRuleNode
    {
        void Activate(IWorkingMemory workingMemory, Tuple tuple);
        void Deactivate(IWorkingMemory workingMemory, Tuple tuple);
    }

    internal class RuleNode : IRuleNode
    {
        private readonly string _ruleHandle;
        private readonly int _rulePriority;

        public RuleNode(string ruleHandle, int rulePriority)
        {
            _ruleHandle = ruleHandle;
            _rulePriority = rulePriority;
        }

        public void Activate(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, _rulePriority, tuple);
            workingMemory.EventAggregator.Activate(activation);
        }

        public void Deactivate(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, _rulePriority, tuple);
            workingMemory.EventAggregator.Deactivate(activation);
        }
    }
}