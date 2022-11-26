using System;
using System.Collections.Generic;
using NRules.Rete;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules;

internal interface IWorkingMemory
{
    IEnumerable<Fact> Facts { get; }

    Fact? GetFact(object factObject);
    void AddFact(Fact fact);
    void UpdateFact(Fact fact);
    void RemoveFact(Fact fact);

    IEnumerable<object> GetLinkedKeys(Activation activation);
    SyntheticFact? GetLinkedFact(Activation activation, object key);
    void AddLinkedFact(Activation activation, object key, SyntheticFact fact);
    void UpdateLinkedFact(Activation activation, object key, SyntheticFact fact, object factObject);
    void RemoveLinkedFact(Activation activation, object key);

    IAlphaMemory GetNodeMemory(IAlphaMemoryNode node);
    IBetaMemory GetNodeMemory(IBetaMemoryNode node);

    T? GetState<T>(INode node, Tuple tuple) where T : class;
    T GetStateOrThrow<T>(INode node, Tuple tuple) where T : class;
    T? RemoveState<T>(INode node, Tuple tuple) where T : class;
    T RemoveStateOrThrow<T>(INode node, Tuple tuple) where T : class;
    void SetState(INode node, Tuple tuple, object value);
}

internal class WorkingMemory : IWorkingMemory
{
    private readonly Dictionary<object, Fact> _factMap = new();
    private readonly Dictionary<Activation, Dictionary<object, SyntheticFact>> _linkedFactMap = new();
    private readonly Dictionary<TupleStateKey, object> _tupleStateMap = new();

    private readonly Dictionary<IAlphaMemoryNode, AlphaMemory> _alphaMap = new();

    private readonly Dictionary<IBetaMemoryNode, BetaMemory> _betaMap = new();

    private static readonly IEnumerable<object> EmptyObjectList = Array.Empty<object>();

    public IEnumerable<Fact> Facts => _factMap.Values;

    public Fact? GetFact(object factObject)
    {
        _factMap.TryGetValue(factObject, out var fact);
        return fact;
    }

    public void AddFact(Fact fact)
    {
        _factMap.Add(fact.RawObject ?? throw new ArgumentException("Fact cannot contain null object"), fact);
    }

    public void UpdateFact(Fact fact)
    {
        RemoveFact(fact);
        AddFact(fact);
    }

    public void RemoveFact(Fact fact)
    {
        if (!_factMap.Remove(fact.RawObject ?? throw new ArgumentException("Fact cannot contain null object")))
            throw new ArgumentException("Element does not exist", nameof(fact));
    }

    public IEnumerable<object> GetLinkedKeys(Activation activation)
    {
        if (!_linkedFactMap.TryGetValue(activation, out var factMap))
            return EmptyObjectList;
        return factMap.Keys;
    }

    public SyntheticFact? GetLinkedFact(Activation activation, object key)
    {
        if (!_linkedFactMap.TryGetValue(activation, out var factMap))
            return null;

        factMap.TryGetValue(key, out var fact);
        return fact;
    }

    public void AddLinkedFact(Activation activation, object key, SyntheticFact fact)
    {
        AddFact(fact);

        var factMap = _linkedFactMap.GetOrAdd(activation, _ => new());
        factMap.Add(key, fact);
    }

    public void UpdateLinkedFact(Activation activation, object key, SyntheticFact fact, object factObject)
    {
        if (!ReferenceEquals(fact.RawObject, factObject))
        {
            RemoveFact(fact);
            fact.RawObject = factObject;
            AddFact(fact);
        }

        var factMap = _linkedFactMap.GetOrAdd(activation, _ => new());
        factMap.Remove(key);
        factMap.Add(key, fact);
    }

    public void RemoveLinkedFact(Activation activation, object key)
    {
        if (!_linkedFactMap.TryGetValue(activation, out var factMap))
            return;

        factMap.Remove(key);
        if (factMap.Count == 0)
            _linkedFactMap.Remove(activation);
    }

    public IAlphaMemory GetNodeMemory(IAlphaMemoryNode node)
    {
        return _alphaMap.GetOrAdd(node, _ => new());
    }

    public IBetaMemory GetNodeMemory(IBetaMemoryNode node)
    {
        return _betaMap.GetOrAdd(node, _ => new());
    }

    public T? GetState<T>(INode node, Tuple tuple) where T : class
    {
        var key = new TupleStateKey(node, tuple);
        if (_tupleStateMap.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return null;
    }

    public T GetStateOrThrow<T>(INode node, Tuple tuple) where T : class
    {
        return GetState<T>(node, tuple)
            ?? throw new ArgumentException($"Tuple state not found. NodeType={node.GetType()}, StateType={typeof(T)}");
    }

    public T? RemoveState<T>(INode node, Tuple tuple) where T : class
    {
        var key = new TupleStateKey(node, tuple);
#if NETSTANDARD2_0
        if (_tupleStateMap.TryGetValue(key, out var value))
        {
            var state = (T)value;
            _tupleStateMap.Remove(key);
            return state;
        }
#else
        if (_tupleStateMap.Remove(key, out var value))
        {
            return (T)value;
        }
#endif
        return null;
    }

    public T RemoveStateOrThrow<T>(INode node, Tuple tuple) where T : class
    {
        return RemoveState<T>(node, tuple)
            ?? throw new ArgumentException($"Tuple state not found. NodeType={node.GetType()}, StateType={typeof(T)}");
    }

    public void SetState(INode node, Tuple tuple, object value)
    {
        var key = new TupleStateKey(node, tuple);
        _tupleStateMap[key] = value;
    }

    private readonly struct TupleStateKey : IEquatable<TupleStateKey>
    {
        private readonly INode _node;
        private readonly Tuple _tuple;

        public TupleStateKey(INode node, Tuple tuple)
        {
            _node = node;
            _tuple = tuple;
        }

        public bool Equals(TupleStateKey other)
        {
            return _node.Equals(other._node) && _tuple.Equals(other._tuple);
        }

        public override bool Equals(object obj)
        {
            return obj is TupleStateKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_node.GetHashCode() * 397) ^ _tuple.GetHashCode();
            }
        }
    }
}