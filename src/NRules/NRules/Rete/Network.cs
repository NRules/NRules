using System;
using System.Collections.Generic;

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
        private readonly List<IActivatable> _activatableNodes;

        public Network(RootNode root, IEnumerable<IActivatable> activatableNodes)
        {
            _root = root;
            _activatableNodes = new List<IActivatable>(activatableNodes);
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
            foreach (var activatableNode in _activatableNodes)
            {
                activatableNode.Activate(context);
            }
        }

        public void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.Visit(context, _root);
        }
    }
}