using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface INetwork
    {
        void PropagateAssert(IExecutionContext context, List<Fact> factObjects);
        void PropagateUpdate(IExecutionContext context, List<Fact> factObjects);
        void PropagateRetract(IExecutionContext context, List<Fact> factObjects);
        void Activate(IExecutionContext context);
        void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }

    internal class Network : INetwork
    {
        private readonly RootNode _root;
        private readonly DummyNode _dummyNode;

        public Network(RootNode root, DummyNode dummyNode)
        {
            _root = root;
            _dummyNode = dummyNode;
        }

        public void PropagateAssert(IExecutionContext context, List<Fact> facts)
        {
            foreach (var fact in facts)
            {
                context.EventAggregator.RaiseFactInserting(context.Session, fact);
            }

            _root.PropagateAssert(context, facts);

            foreach (var fact in facts)
            {
                context.EventAggregator.RaiseFactInserted(context.Session, fact);
            }
        }

        public void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            foreach (var fact in facts)
            {
                context.EventAggregator.RaiseFactUpdating(context.Session, fact);
            }

            _root.PropagateUpdate(context, facts);

            foreach (var fact in facts)
            {
                context.EventAggregator.RaiseFactUpdated(context.Session, fact);
            }
        }

        public void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            foreach (var fact in facts)
            {
                context.EventAggregator.RaiseFactRetracting(context.Session, fact);
            }

            _root.PropagateRetract(context, facts);

            foreach (var fact in facts)
            {
                context.EventAggregator.RaiseFactRetracted(context.Session, fact);
            }
        }

        public void Activate(IExecutionContext context)
        {
            _dummyNode.Activate(context);
        }

        public void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.Visit(context, _root);
        }
    }
}