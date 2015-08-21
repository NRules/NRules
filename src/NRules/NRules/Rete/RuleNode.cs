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
        public ICompiledRule Rule { get; private set; }

        public RuleNode(ICompiledRule rule)
        {
            Rule = rule;
        }

        public void Activate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            int priority = Rule.Priority.Invoke(context, Rule, tuple, tupleFactMap);
            tuple.SetState(this, priority);
            var activation = new Activation(Rule, priority, tuple, tupleFactMap);
            context.Agenda.Activate(activation);
            context.EventAggregator.RaiseActivationCreated(context.Session, activation);
        }

        public void Reactivate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            int priority = tuple.GetState<int>(this);
            var activation = new Activation(Rule, priority, tuple, tupleFactMap);
            context.Agenda.Reactivate(activation);
            context.EventAggregator.RaiseActivationUpdated(context.Session, activation);
        }

        public void Deactivate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            int priority = tuple.GetState<int>(this);
            var activation = new Activation(Rule, priority, tuple, tupleFactMap);
            context.Agenda.Deactivate(activation);
            context.EventAggregator.RaiseActivationDeleted(context.Session, activation);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRuleNode(context, this);
        }
    }
}