using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IRuleNode : INode
    {
        void PropagateAssert(IExecutionContext context, List<Tuple> tuples);
        void PropagateUpdate(IExecutionContext context, List<Tuple> tuples);
        void PropagateRetract(IExecutionContext context, List<Tuple> tuples);
    }

    internal class RuleNode : IRuleNode, ITupleSink
    {
        public ICompiledRule CompiledRule { get; }

        public RuleNode(ICompiledRule compiledRule)
        {
            CompiledRule = compiledRule;
        }

        public void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                var activation = new Activation(CompiledRule, tuple);
                context.WorkingMemory.SetState(this, tuple, activation);
                context.Agenda.Add(activation);
                context.EventAggregator.RaiseActivationCreated(context.Session, activation);
            }
        }

        public void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                var activation = context.WorkingMemory.GetStateOrThrow<Activation>(this, tuple);
                context.Agenda.Modify(activation);
                context.EventAggregator.RaiseActivationUpdated(context.Session, activation);
            }
        }

        public void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            foreach (var tuple in tuples)
            {
                var activation = context.WorkingMemory.RemoveStateOrThrow<Activation>(this, tuple);
                context.Agenda.Remove(activation);
                context.EventAggregator.RaiseActivationDeleted(context.Session, activation);
            }
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitRuleNode(context, this);
        }
    }
}