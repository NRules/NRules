namespace NRules.Rete
{
    internal interface IRuleNode : INode
    {
        void Activate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap);
        void Reactivate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap);
        void Deactivate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap);
    }

    internal class RuleNode : IRuleNode
    {
        public ICompiledRule CompiledRule { get; private set; }

        public RuleNode(ICompiledRule compiledRule)
        {
            CompiledRule = compiledRule;
        }

        public void Activate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            var activation = new Activation(CompiledRule, tuple, tupleFactMap);
            context.Agenda.Add(activation);
            context.EventAggregator.RaiseActivationCreated(context.Session, activation);
        }

        public void Reactivate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            var activation = new Activation(CompiledRule, tuple, tupleFactMap);
            context.Agenda.Modify(activation);
            context.EventAggregator.RaiseActivationUpdated(context.Session, activation);
        }

        public void Deactivate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            var activation = new Activation(CompiledRule, tuple, tupleFactMap);
            context.Agenda.Remove(activation);
            context.EventAggregator.RaiseActivationDeleted(context.Session, activation);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRuleNode(context, this);
        }
    }
}