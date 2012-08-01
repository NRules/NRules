using System;

namespace NRules.Core.Rete
{
    internal interface INetwork
    {
        void PropagateAssert(IWorkingMemory workingMemory, object factObject);
        void PropagateUpdate(IWorkingMemory workingMemory, object factObject);
        void PropagateRetract(IWorkingMemory workingMemory, object factObject);
    }

    internal class Network : INetwork
    {
        private readonly RootNode _root;

        public Network(RootNode root)
        {
            _root = root;
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
    }
}