using System;
using System.Collections.Generic;
using NRules.Rete;
using NRules.RuleModel;
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

    IEnumerable<object> GetLinkedKeys(IMatch activation);
    Fact? GetLinkedFact(IMatch activation, object key);
    void AddLinkedFact(IMatch activation, object key, Fact fact);
    void UpdateLinkedFact(IMatch activation, object key, Fact fact, object factObject);
    void RemoveLinkedFact(IMatch activation, object key, Fact fact);

    IAlphaMemory GetNodeMemory(IAlphaMemoryNode node);
    IBetaMemory GetNodeMemory(IBetaMemoryNode node);

    T? GetState<T>(INode node, Tuple tuple);
    T GetStateOrThrow<T>(INode node, Tuple tuple);
    T? RemoveState<T>(INode node, Tuple tuple);
    T RemoveStateOrThrow<T>(INode node, Tuple tuple);
    void SetState(INode node, Tuple tuple, object value);
}

internal class WorkingMemory : IWorkingMemory
{
    private readonly Dictionary<object, Fact> _factMap = new();
    private readonly Dictionary<IMatch, Dictionary<object, Fact>> _linkedFactMap = new();
    private readonly Dictionary<(INode Node, Tuple Tuple), object> _tupleStateMap = new();
    private readonly Dictionary<IAlphaMemoryNode, IAlphaMemory> _alphaMap = new();
    private readonly Dictionary<IBetaMemoryNode, IBetaMemory> _betaMap = new();

    private static readonly IEnumerable<object> EmptyObjectList = Array.Empty<object>();

    public IEnumerable<Fact> Facts => _factMap.Values;

    public Fact? GetFact(object factObject)
    {
        return _factMap.GetValueOrDefault(factObject);
    }

    public void AddFact(Fact fact)
    {
        if (fact.RawObject is null)
        {
            throw new ArgumentException($"{nameof(fact.RawObject)} is null", nameof(fact));
        }

        _factMap.Add(fact.RawObject, fact);
    }

    public void UpdateFact(Fact fact)
    {
        RemoveFact(fact);
        AddFact(fact);
    }

    public void RemoveFact(Fact fact)
    {
        if (fact.RawObject is null)
        {
            throw new ArgumentException($"{nameof(fact.RawObject)} is null", nameof(fact));
        }

        if (!_factMap.Remove(fact.RawObject))
            throw new ArgumentException("Element does not exist", nameof(fact));
    }

    public IEnumerable<object> GetLinkedKeys(IMatch activation)
    {
        return _linkedFactMap.GetValueOrDefault(activation)?.Keys ?? EmptyObjectList;
    }

    public Fact? GetLinkedFact(IMatch activation, object key)
    {
        return _linkedFactMap.GetValueOrDefault(activation)?.GetValueOrDefault(key);
    }

    public void AddLinkedFact(IMatch activation, object key, Fact fact)
    {
        AddFact(fact);
        _linkedFactMap.GetOrAdd(activation, _ => new()).Add(key, fact);
    }

    public void UpdateLinkedFact(IMatch activation, object key, Fact fact, object factObject)
    {
        if (!ReferenceEquals(fact.RawObject, factObject))
        {
            RemoveFact(fact);
            fact.RawObject = factObject;
            AddFact(fact);
        }

        _linkedFactMap.GetOrAdd(activation, _ => new())[key] = fact;
    }

    public void RemoveLinkedFact(IMatch activation, object key, Fact fact)
    {
        var factMap = _linkedFactMap.GetValueOrDefault(activation);
        factMap?.Remove(key);
        if (factMap?.Count == 0)
            _linkedFactMap.Remove(activation);
    }

    public IAlphaMemory GetNodeMemory(IAlphaMemoryNode node)
    {
        return _alphaMap.GetOrAdd(node, _ => new AlphaMemory());
    }

    public IBetaMemory GetNodeMemory(IBetaMemoryNode node)
    {
        return _betaMap.GetOrAdd(node, _ => new BetaMemory());
    }

    public T? GetState<T>(INode node, Tuple tuple)
    {
        return (T?)_tupleStateMap.GetValueOrDefault((node, tuple));
    }

    public T GetStateOrThrow<T>(INode node, Tuple tuple)
    {
        return GetState<T>(node, tuple)
            ?? throw new ArgumentException($"Tuple state not found. NodeType={node.GetType()}, StateType={typeof(T)}");
    }

    public T? RemoveState<T>(INode node, Tuple tuple)
    {
        return (T?)_tupleStateMap.TryRemoveAndGetValue((node, tuple));
    }

    public T RemoveStateOrThrow<T>(INode node, Tuple tuple)
    {
        return RemoveState<T>(node, tuple)
            ?? throw new ArgumentException($"Tuple state not found. NodeType={node.GetType()}, StateType={typeof(T)}");
    }

    public void SetState(INode node, Tuple tuple, object value)
    {
        _tupleStateMap[(node, tuple)] = value;
    }
}