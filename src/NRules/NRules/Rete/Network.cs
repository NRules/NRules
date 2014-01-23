using System;
using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface INetwork
    {
        void PropagateAssert(IWorkingMemory workingMemory, object factObject);
        void PropagateUpdate(IWorkingMemory workingMemory, object factObject);
        void PropagateRetract(IWorkingMemory workingMemory, object factObject);
        void Activate(IWorkingMemory workingMemory);
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

        public void PropagateAssert(IWorkingMemory workingMemory, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = workingMemory.GetFact(factObject);
            if (fact != null)
            {
                throw new ArgumentException("Fact for insert already exists", "factObject");
            }
            fact = new Fact(factObject);
            workingMemory.SetFact(fact);
            _root.PropagateAssert(workingMemory, fact);
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = workingMemory.GetFact(factObject);
            if (fact == null)
            {
                throw new ArgumentException("Fact for update does not exist", "factObject");
            }
            _root.PropagateUpdate(workingMemory, fact);
        }

        public void PropagateRetract(IWorkingMemory workingMemory, object factObject)
        {
            if (factObject == null)
            {
                throw new ArgumentNullException("factObject");
            }
            Fact fact = workingMemory.GetFact(factObject);
            if (fact == null)
            {
                throw new ArgumentException("Fact for retract does not exist", "factObject");
            }
            _root.PropagateRetract(workingMemory, fact);
            workingMemory.RemoveFact(fact);
        }

        public void Activate(IWorkingMemory workingMemory)
        {
            foreach (var activatedNode in _activatableNodes)
            {
                activatedNode.Activate(workingMemory);
            }
        }
    }
}