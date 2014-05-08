namespace NRules.Rete
{
    internal interface IRuleNode : INode
    {
        void Activate(IExecutionContext context, Tuple tuple);
        void Deactivate(IExecutionContext context, Tuple tuple);
    }

    internal class RuleNode : IRuleNode
    {
        public ICompiledRule Rule { get; private set; }

        public RuleNode(ICompiledRule rule)
        {
            Rule = rule;
        }

        public void Activate(IExecutionContext context, Tuple tuple)
        {
            var activation = new Activation(Rule, tuple);
            context.Agenda.Activate(activation);
            context.EventAggregator.ActivationCreated(activation);
        }

        public void Deactivate(IExecutionContext context, Tuple tuple)
        {
            var activation = new Activation(Rule, tuple);
            context.Agenda.Deactivate(activation);
            context.EventAggregator.ActivationDeleted(activation);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRuleNode(context, this);
        }
    }
}