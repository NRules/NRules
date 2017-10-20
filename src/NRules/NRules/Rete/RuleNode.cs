namespace NRules.Rete
{
    internal interface IRuleNode : INode
    {
        void PropagateAssert(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap);
        void PropagateUpdate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap);
        void PropagateRetract(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap);
    }

    internal class RuleNode : IRuleNode
    {
        public ICompiledRule CompiledRule { get; }

        public RuleNode(ICompiledRule compiledRule)
        {
            CompiledRule = compiledRule;
        }

        public void PropagateAssert(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            var activation = new Activation(CompiledRule, tuple, tupleFactMap);
            tuple.SetState(this, activation);
            context.Agenda.Add(context, activation);
            context.EventAggregator.RaiseActivationCreated(context.Session, activation);
        }

        public void PropagateUpdate(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            var activation = tuple.GetState<Activation>(this);
            context.Agenda.Modify(context, activation);
            context.EventAggregator.RaiseActivationUpdated(context.Session, activation);
        }

        public void PropagateRetract(IExecutionContext context, Tuple tuple, IndexMap tupleFactMap)
        {
            var activation = tuple.RemoveState<Activation>(this);
            context.Agenda.Remove(context, activation);
            context.EventAggregator.RaiseActivationDeleted(context.Session, activation);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRuleNode(context, this);
        }
    }
}