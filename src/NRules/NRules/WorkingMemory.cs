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

        IEnumerable<object> GetLinkedKeys(Activation activation);
        Fact GetLinkedFact(Activation activation, object key);
        void AddLinkedFact(Activation activation, object key, Fact fact);
        void UpdateLinkedFact(Activation activation, object key, Fact fact, object factObject);
        void RemoveLinkedFact(Activation activation, object key, Fact fact);

        IAlphaMemory GetNodeMemory(IAlphaMemoryNode node);
        IBetaMemory GetNodeMemory(IBetaMemoryNode node);
    }

    internal class WorkingMemory : IWorkingMemory
    {
        private readonly Dictionary<object, Fact> _factMap = new Dictionary<object, Fact>();
        private readonly Dictionary<Activation, Dictionary<object, Fact>> _linkedFactMap = new Dictionary<Activation, Dictionary<object, Fact>>();

        private readonly Dictionary<IAlphaMemoryNode, IAlphaMemory> _alphaMap =
            new Dictionary<IAlphaMemoryNode, IAlphaMemory>();

        private readonly Dictionary<IBetaMemoryNode, IBetaMemory> _betaMap =
            new Dictionary<IBetaMemoryNode, IBetaMemory>();

        private static readonly object[] EmptyObjectList = new object[0];

        public IEnumerable<Fact> Facts => _factMap.Values;

        public Fact GetFact(object factObject)
        {
            _factMap.TryGetValue(factObject, out var fact);
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

        public IEnumerable<object> GetLinkedKeys(Activation activation)
        {
            if (!_linkedFactMap.TryGetValue(activation, out var factMap)) return EmptyObjectList;
            return factMap.Keys;
        }

        public Fact GetLinkedFact(Activation activation, object key)
        {
            if (!_linkedFactMap.TryGetValue(activation, out var factMap)) return null;

            factMap.TryGetValue(key, out var fact);
            return fact;
        }

        public void AddLinkedFact(Activation activation, object key, Fact fact)
        {
            AddFact(fact);

            if (!_linkedFactMap.TryGetValue(activation, out var factMap))
            {
                factMap = new Dictionary<object, Fact>();
                _linkedFactMap[activation] = factMap;
            }

            factMap.Add(key, fact);
        }

        public void UpdateLinkedFact(Activation activation, object key, Fact fact, object factObject)
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

        public void RemoveLinkedFact(Activation activation, object key, Fact fact)
        {
            if (!_linkedFactMap.TryGetValue(activation, out var factMap)) return;

            factMap.Remove(key);
            if (factMap.Count == 0) _linkedFactMap.Remove(activation);
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