namespace NRules.Rete
{
    internal interface IRuleNode
    {
        void Activate(IWorkingMemory workingMemory, Tuple tuple);
        void Deactivate(IWorkingMemory workingMemory, Tuple tuple);
    }

    internal class RuleNode : IRuleNode
    {
        private readonly ICompiledRule _rule;

        public RuleNode(ICompiledRule rule)
        {
            _rule = rule;
        }

        public void Activate(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_rule, tuple);
            workingMemory.EventAggregator.Activate(activation);
        }

        public void Deactivate(IWorkingMemory workingMemory, Tuple tuple)
        {
            var activation = new Activation(_rule, tuple);
            workingMemory.EventAggregator.Deactivate(activation);
        }
    }
}