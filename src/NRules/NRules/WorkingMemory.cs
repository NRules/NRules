using System;
using System.Collections.Generic;
using NRules.Rete;

namespace NRules
{
    internal interface IWorkingMemory
    {
        IEnumerable<Fact> Facts { get; }

        Fact GetFact(object factObject);
        void AddFact(Fact fact);
        void UpdateFact(Fact fact);
        void RemoveFact(Fact fact);

        Fact GetInternalFact(INode node, object factObject);
        IEnumerable<Fact> GetInternalFacts(INode node, IEnumerable<object> factObjects);
        void AddInternalFact(INode node, Fact fact);
        void UpdateInternalFact(INode node, Fact fact);
        void RemoveInternalFact(INode node, Fact fact);

        IEnumerable<object> GetLinkedKeys(IActivation activation);
        Fact GetLinkedFact(IActivation activation, object key);
        void AddLinkedFact(IActivation activation, object key, Fact fact);
        void UpdateLinkedFact(IActivation activation, object key, Fact fact, object factObject);
        void RemoveLinkedFact(IActivation activation, object key, Fact fact);

        IAlphaMemory GetNodeMemory(IAlphaMemoryNode node);
        IBetaMemory GetNodeMemory(IBetaMemoryNode node);
    }

    internal class WorkingMemory : IWorkingMemory
    {
        private readonly Dictionary<object, Fact> _factMap = new Dictionary<object, Fact>();
        private readonly Dictionary<INode, Dictionary<object, Fact>> _internalFactMap = new Dictionary<INode, Dictionary<object, Fact>>();
        private readonly Dictionary<IActivation, Dictionary<object, Fact>> _linkedFactMap = new Dictionary<IActivation, Dictionary<object, Fact>>();

        private readonly Dictionary<IAlphaMemoryNode, IAlphaMemory> _alphaMap =
            new Dictionary<IAlphaMemoryNode, IAlphaMemory>();

        private readonly Dictionary<IBetaMemoryNode, IBetaMemory> _betaMap =
            new Dictionary<IBetaMemoryNode, IBetaMemory>();

        private static readonly Fact[] EmptyFactList = new Fact[0];
        private static readonly object[] EmptyObjectList = new object[0];

        public IEnumerable<Fact> Facts => _factMap.Values;

        public Fact GetFact(object factObject)
        {
            Fact fact;
            _factMap.TryGetValue(factObject, out fact);
            return fact;
        }

        public void AddFact(Fact fact)
        {
            _factMap.Add(fact.RawObject, fact);
        }

        public void UpdateFact(Fact fact)
        {
            RemoveFact(fact);
            AddFact(fact);
        }

        public void RemoveFact(Fact fact)
        {
            if (!_factMap.Remove(fact.RawObject))
            {
                throw new ArgumentException("Element does not exist", nameof(fact));
            }
        }

        public Fact GetInternalFact(INode node, object factObject)
        {
            if (!_internalFactMap.TryGetValue(node, out var factMap)) return null;

            factMap.TryGetValue(factObject, out var fact);
            return fact;
        }

        public IEnumerable<Fact> GetInternalFacts(INode node, IEnumerable<object> factObjects)
        {
            if (!_internalFactMap.TryGetValue(node, out var factMap)) return EmptyFactList;

            var facts = new List<Fact>();
            foreach (var factObject in factObjects)
            {
                factMap.TryGetValue(factObject, out var fact);
                facts.Add(fact);
            }
            return facts;
        }

        public void AddInternalFact(INode node, Fact fact)
        {
            if (!_internalFactMap.TryGetValue(node, out var factMap))
            {
                factMap = new Dictionary<object, Fact>();
                _internalFactMap[node] = factMap;
            }

            factMap[fact.RawObject] = fact;
        }

        public void UpdateInternalFact(INode node, Fact fact)
        {
            if (!_internalFactMap.TryGetValue(node, out var factMap))
            {
                factMap = new Dictionary<object, Fact>();
                _internalFactMap[node] = factMap;
            }

            factMap.Remove(fact.RawObject);
            factMap[fact.RawObject] = fact;
        }

        public void RemoveInternalFact(INode node, Fact fact)
        {
            if (!_internalFactMap.TryGetValue(node, out var factMap)) return;

            factMap.Remove(fact.RawObject);
            if (factMap.Count == 0) _internalFactMap.Remove(node);
        }

        public IEnumerable<object> GetLinkedKeys(IActivation activation)
        {
            if (!_linkedFactMap.TryGetValue(activation, out var factMap)) return EmptyObjectList;
            return factMap.Keys;
        }

        public Fact GetLinkedFact(IActivation activation, object key)
        {
            if (!_linkedFactMap.TryGetValue(activation, out var factMap)) return null;

            factMap.TryGetValue(key, out var fact);
            return fact;
        }

        public void AddLinkedFact(IActivation activation, object key, Fact fact)
        {
            AddFact(fact);

            if (!_linkedFactMap.TryGetValue(activation, out var factMap))
            {
                factMap = new Dictionary<object, Fact>();
                _linkedFactMap[activation] = factMap;
            }

            factMap.Add(key, fact);
        }

        public void UpdateLinkedFact(IActivation activation, object key, Fact fact, object factObject)
        {
            if (!ReferenceEquals(fact.RawObject, factObject))
            {
                RemoveFact(fact);
                fact.RawObject = factObject;
                AddFact(fact);
            }

            if (!_linkedFactMap.TryGetValue(activation, out var factMap))
            {
                factMap = new Dictionary<object, Fact>();
                _linkedFactMap[activation] = factMap;
            }

            factMap.Remove(key);
            factMap.Add(key, fact);
        }

        public void RemoveLinkedFact(IActivation activation, object key, Fact fact)
        {
            if (!_linkedFactMap.TryGetValue(activation, out var factMap)) return;

            factMap.Remove(key);
            if (factMap.Count == 0) _linkedFactMap.Remove(activation);

            RemoveFact(fact);
        }

        public IAlphaMemory GetNodeMemory(IAlphaMemoryNode node)
        {
            if (!_alphaMap.TryGetValue(node, out var memory))
            {
                memory = new AlphaMemory();
                _alphaMap[node] = memory;
            }
            return memory;
        }

        public IBetaMemory GetNodeMemory(IBetaMemoryNode node)
        {
            if (!_betaMap.TryGetValue(node, out var memory))
            {
                memory = new BetaMemory();
                _betaMap[node] = memory;
            }
            return memory;
        }
    }
}