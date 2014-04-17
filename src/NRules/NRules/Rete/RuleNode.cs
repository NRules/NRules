namespace NRules.Rete
{
    internal interface IRuleNode
    {
        void Activate(IExecutionContext context, Tuple tuple);
        void Deactivate(IExecutionContext context, Tuple tuple);
    }

    internal class RuleNode : IRuleNode
    {
        private readonly ICompiledRule _rule;

        public RuleNode(ICompiledRule rule)
        {
            _rule = rule;
        }

        public void Activate(IExecutionContext context, Tuple tuple)
        {
            var activation = new Activation(_rule, tuple);
            context.Agenda.Activate(activation);
            context.EventAggregator.ActivationCreated(activation);
        }

        public void Deactivate(IExecutionContext context, Tuple tuple)
        {
            var activation = new Activation(_rule, tuple);
            context.Agenda.Deactivate(activation);
            context.EventAggregator.ActivationDeleted(activation);
        }
    }
}