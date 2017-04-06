using System;
using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface INetwork
    {
        FactResult PropagateAssert(IExecutionContext context, IEnumerable<object> factObjects);
        FactResult PropagateUpdate(IExecutionContext context, IEnumerable<object> factObjects);
        FactResult PropagateRetract(IExecutionContext context, IEnumerable<object> factObjects);
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

        public FactResult PropagateAssert(IExecutionContext context, IEnumerable<object> factObjects)
        {
            if (factObjects == null)
            {
                throw new ArgumentNullException("factObjects");
            }

            var failed = new List<object>();
            var toPropagate = new List<Fact>();
            foreach (var factObject in factObjects)
            {
                var fact = context.WorkingMemory.GetFact(factObject);
                if (fact == null)
                {
                    fact = new Fact(factObject);
                    toPropagate.Add(fact);
                }
                else
                {
                    failed.Add(factObject);
                }
            }

            var result = new FactResult(failed);
            if (result.FailedCount == 0)
            {
                foreach (var fact in toPropagate)
                {
                    context.EventAggregator.RaiseFactInserting(context.Session, fact);
                    context.WorkingMemory.AddFact(fact);
                }

                _root.PropagateAssert(context, toPropagate);

                foreach (var fact in toPropagate)
                {
                    context.EventAggregator.RaiseFactInserted(context.Session, fact);
                }
            }
            return result;
        }

        public FactResult PropagateUpdate(IExecutionContext context, IEnumerable<object> factObjects)
        {
            if (factObjects == null)
            {
                throw new ArgumentNullException("factObjects");
            }

            var failed = new List<object>();
            var toPropagate = new List<Fact>();
            foreach (var factObject in factObjects)
            {
                var fact = context.WorkingMemory.GetFact(factObject);
                if (fact != null)
                {
                    UpdateFact(fact, factObject);
                    toPropagate.Add(fact);
                }
                else
                {
                    failed.Add(factObject);
                }
            }

            var result = new FactResult(failed);
            if (result.FailedCount == 0)
            {
                foreach (var fact in toPropagate)
                {
                    context.WorkingMemory.UpdateFact(fact);
                    context.EventAggregator.RaiseFactUpdating(context.Session, fact);
                }

                _root.PropagateUpdate(context, toPropagate);

                foreach (var fact in toPropagate)
                {
                    context.EventAggregator.RaiseFactUpdated(context.Session, fact);
                }
            }
            return result;
        }

        public FactResult PropagateRetract(IExecutionContext context, IEnumerable<object> factObjects)
        {
            if (factObjects == null)
            {
                throw new ArgumentNullException("factObjects");
            }

            var failed = new List<object>();
            var toPropagate = new List<Fact>();
            foreach (var factObject in factObjects)
            {
                var fact = context.WorkingMemory.GetFact(factObject);
                if (fact != null)
                {
                    toPropagate.Add(fact);
                }
                else
                {
                    failed.Add(factObject);
                }
            }

            var result = new FactResult(failed);
            if (result.FailedCount == 0)
            {
                foreach (var fact in toPropagate)
                {
                    context.EventAggregator.RaiseFactRetracting(context.Session, fact);
                }

                _root.PropagateRetract(context, toPropagate);

                foreach (var fact in toPropagate)
                {
                    context.WorkingMemory.RemoveFact(fact);
                    context.EventAggregator.RaiseFactRetracted(context.Session, fact);
                }
            }
            return result;
        }

        public void Activate(IExecutionContext context)
        {
            _dummyNode.Activate(context);
        }

        public void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.Visit(context, _root);
        }

        private static void UpdateFact(Fact fact, object factObject)
        {
            if (ReferenceEquals(fact.RawObject, factObject)) return;
            fact.RawObject = factObject;
        }
    }
}