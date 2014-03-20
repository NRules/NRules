using System.Collections.Generic;
using NRules.Rete;

namespace NRules
{
    internal interface IWorkingMemory
    {
        IEventAggregator EventAggregator { get; }
        IEnumerable<Fact> Facts { get; }
        Fact GetFact(object factObject);
        void SetFact(Fact fact);
        void RemoveFact(Fact fact);
        IAlphaMemory GetNodeMemory(IAlphaMemoryNode node);
        IBetaMemory GetNodeMemory(IBetaMemoryNode node);
        object GetNodeState(BetaNode node);
        void SetNodeState(BetaNode node, object state);
    }

    internal class WorkingMemory : IWorkingMemory
    {
        private readonly Dictionary<object, Fact> _factMap = new Dictionary<object, Fact>();
        private readonly Dictionary<BetaNode, object> _betaNodeStates = new Dictionary<BetaNode, object>();

        private readonly Dictionary<IAlphaMemoryNode, IAlphaMemory> _alphaMap =
            new Dictionary<IAlphaMemoryNode, IAlphaMemory>();

        private readonly Dictionary<IBetaMemoryNode, IBetaMemory> _betaMap =
            new Dictionary<IBetaMemoryNode, IBetaMemory>();

        public IEventAggregator EventAggregator { get; private set; }
        public IEnumerable<Fact> Facts { get { return _factMap.Values; } }

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
            }
            return memory;
        }

        public object GetNodeState(BetaNode node)
        {
            object state;
            _betaNodeStates.TryGetValue(node, out state);
            return state;
        }

        public void SetNodeState(BetaNode node, object state)
        {
            _betaNodeStates[node] = state;
        }
    }
}