using System.Collections.Generic;
using NRules.Core.Rete;

namespace NRules.Core
{
    internal interface IWorkingMemory
    {
        IEventAggregator EventAggregator { get; }
        Fact GetFact(object factObject);
        void SetFact(Fact fact);
        void RemoveFact(Fact fact);
        IAlphaMemory GetNodeMemory(IAlphaMemoryNode node);
        IBetaMemory GetNodeMemory(IBetaMemoryNode node);
    }

    internal class WorkingMemory : IWorkingMemory
    {
        private readonly Dictionary<object, Fact> _factMap = new Dictionary<object, Fact>();

        private readonly Dictionary<IAlphaMemoryNode, IAlphaMemory> _alphaMap =
            new Dictionary<IAlphaMemoryNode, IAlphaMemory>();

        private readonly Dictionary<IBetaMemoryNode, IBetaMemory> _betaMap =
            new Dictionary<IBetaMemoryNode, IBetaMemory>();

        public IEventAggregator EventAggregator { get; private set; }

        public WorkingMemory(IEventAggregator aggregator)
        {
            EventAggregator = aggregator;
        }

        public Fact GetFact(object factObject)
        {
            Fact fact;
            _factMap.TryGetValue(factObject, out fact);
            return fact;
        }

        public void SetFact(Fact fact)
        {
            _factMap[fact.Object] = fact;
        }

        public void RemoveFact(Fact fact)
        {
            _factMap.Remove(fact.Object);
        }

        public IAlphaMemory GetNodeMemory(IAlphaMemoryNode node)
        {
            IAlphaMemory memory;
            if (!_alphaMap.TryGetValue(node, out memory))
            {
                memory = new AlphaMemory();
                _alphaMap[node] = memory;
            }
            return memory;
        }

        public IBetaMemory GetNodeMemory(IBetaMemoryNode node)
        {
            IBetaMemory memory;
            if (!_betaMap.TryGetValue(node, out memory))
            {
                memory = new BetaMemory();
                _betaMap[node] = memory;
                node.InitializeMemory(memory);
            }
            return memory;
        }
    }
}