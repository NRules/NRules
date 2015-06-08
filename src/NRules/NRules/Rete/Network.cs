using System;

namespace NRules.Rete
{
    internal interface INetwork
    {
        bool PropagateAssert(IExecutionContext context, object factObject);
        bool PropagateUpdate(IExecutionContext context, object factObject);
        bool PropagateRetract(IExecutionContext context, object factObject);
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

        public bool PropagateAssert(IExecutionContext context, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = context.WorkingMemory.GetFact(factObject);
            if (fact != null) return false;
            fact = new Fact(factObject);
            context.EventAggregator.RaiseFactInserting(context.Session, fact);
            context.WorkingMemory.SetFact(fact);
            _root.PropagateAssert(context, fact);
            context.EventAggregator.RaiseFactInserted(context.Session, fact);
            return true;
        }

        public bool PropagateUpdate(IExecutionContext context, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = context.WorkingMemory.GetFact(factObject);
            if (fact == null)
            {
                return false;
            }
            UpdateFact(context, fact, factObject);
            context.EventAggregator.RaiseFactUpdating(context.Session, fact);
            _root.PropagateUpdate(context, fact);
            context.EventAggregator.RaiseFactUpdated(context.Session, fact);
            return true;
        }

        public bool PropagateRetract(IExecutionContext context, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = context.WorkingMemory.GetFact(factObject);
            if (fact == null)
            {
                return false;
            }
            context.EventAggregator.RaiseFactRetracting(context.Session, fact);
            _root.PropagateRetract(context, fact);
            context.WorkingMemory.RemoveFact(fact);
            context.EventAggregator.RaiseFactRetracted(context.Session, fact);
            return true;
        }

        public void Activate(IExecutionContext context)
        {
            _dummyNode.Activate(context);
        }

        public void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.Visit(context, _root);
        }

        private static void UpdateFact(IExecutionContext context, Fact fact, object factObject)
        {
            if (ReferenceEquals(fact.RawObject, factObject)) return;

            fact.RawObject = factObject;
            context.WorkingMemory.SetFact(fact);
        }
    }
}