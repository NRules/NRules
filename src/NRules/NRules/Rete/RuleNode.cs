namespace NRules.Rete
{
    internal interface IRuleNode : INode
    {
        void PropagateAssert(IExecutionContext context, Tuple tuple, IndexMap factMap);
        void PropagateUpdate(IExecutionContext context, Tuple tuple, IndexMap factMap);
        void PropagateRetract(IExecutionContext context, Tuple tuple, IndexMap factMap);
    }

    internal class RuleNode : IRuleNode
    {
        public ICompiledRule CompiledRule { get; }

        public RuleNode(ICompiledRule compiledRule)
        {
            CompiledRule = compiledRule;
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple, IndexMap factMap)
        {
            var activation = new Activation(CompiledRule, tuple, factMap);
            tuple.SetState(this, activation);
            context.Agenda.Add(context, activation);
            context.EventAggregator.RaiseActivationCreated(context.Session, activation);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple, IndexMap factMap)
        {
            var activation = tuple.GetStateOrThrow<Activation>(this);
            context.Agenda.Modify(context, activation);
            context.EventAggregator.RaiseActivationUpdated(context.Session, activation);
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple, IndexMap factMap)
        {
            var activation = tuple.RemoveStateOrThrow<Activation>(this);
            context.Agenda.Remove(context, activation);
            context.EventAggregator.RaiseActivationDeleted(context.Session, activation);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRuleNode(context, this);
        }
    }
}