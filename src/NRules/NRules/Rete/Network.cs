using System;

namespace NRules.Rete
{
    internal interface INetwork
    {
        void PropagateAssert(IExecutionContext context, object factObject);
        void PropagateUpdate(IExecutionContext context, object factObject);
        void PropagateRetract(IExecutionContext context, object factObject);
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

        public void PropagateAssert(IExecutionContext context, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = context.WorkingMemory.GetFact(factObject);
            if (fact != null)
            {
                throw new ArgumentException("Fact for insert already exists", "factObject");
            }
            fact = new Fact(factObject);
            context.WorkingMemory.SetFact(fact);
            context.EventAggregator.FactInserted(fact);
            _root.PropagateAssert(context, fact);
        }

        public void PropagateUpdate(IExecutionContext context, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = context.WorkingMemory.GetFact(factObject);
            if (fact == null)
            {
                throw new ArgumentException("Fact for update does not exist", "factObject");
            }
            context.EventAggregator.FactUpdated(fact);
            _root.PropagateUpdate(context, fact);
        }

        public void PropagateRetract(IExecutionContext context, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = context.WorkingMemory.GetFact(factObject);
            if (fact == null)
            {
                throw new ArgumentException("Fact for retract does not exist", "factObject");
            }
            context.EventAggregator.FactRetracted(fact);
            _root.PropagateRetract(context, fact);
            context.WorkingMemory.RemoveFact(fact);
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