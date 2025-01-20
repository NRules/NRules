using System;
using System.Collections.Generic;

namespace NRules.Rete;

internal interface IBetaMemory
{
    IReadOnlyCollection<Tuple> Tuples { get; }
    void Add(List<Tuple> tuples);
    void Remove(List<Tuple> tuples);
    Tuple? FindTuple(Tuple leftTuple, Fact? rightFact);
    bool Contains(Tuple leftTuple, Fact? rightFact);
}

internal class BetaMemory : IBetaMemory
{
    private readonly HashSet<Tuple> _tuples = new();
    private readonly Dictionary<TupleFactKey, Tuple> _parentToChildMap = new(); 

    public IReadOnlyCollection<Tuple> Tuples => _tuples;

    public void Add(List<Tuple> tuples)
    {
        foreach (var tuple in tuples)
        {
            _tuples.Add(tuple);
            AddMapping(tuple);
        }
    }

    public void Remove(List<Tuple> tuples)
    {
        foreach (var tuple in tuples)
        {
            _tuples.Remove(tuple);
            RemoveMapping(tuple);
        }
    }

    public Tuple? FindTuple(Tuple leftTuple, Fact? rightFact)
    {
        var key = new TupleFactKey(leftTuple, rightFact);
        _parentToChildMap.TryGetValue(key, out var childTuple);
        return childTuple;
    }

    public bool Contains(Tuple leftTuple, Fact? rightFact)
    {
        var key = new TupleFactKey(leftTuple, rightFact);
        return _parentToChildMap.ContainsKey(key);
    }

    private void AddMapping(Tuple tuple)
    {
        if (tuple.LeftTuple == null) return;
        var key = new TupleFactKey(tuple.LeftTuple, tuple.RightFact);
        _parentToChildMap[key] = tuple;
    }

    private void RemoveMapping(Tuple tuple)
    {
        if (tuple.LeftTuple == null) return;
        var key = new TupleFactKey(tuple.LeftTuple, tuple.RightFact);
        _parentToChildMap.Remove(key);
    }

    private readonly struct TupleFactKey(Tuple tuple, Fact? fact) : IEquatable<TupleFactKey>
    {
        private readonly Tuple _tuple = tuple;
        private readonly Fact? _fact = fact;

        public bool Equals(TupleFactKey other)
        {
            return ReferenceEquals(_tuple, other._tuple) && ReferenceEquals(_fact, other._fact);
        }

        public override bool Equals(object? obj)
        {
            return obj is TupleFactKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_tuple.GetHashCode() * 397) ^ (_fact != null ? _fact.GetHashCode() : 0);
            }
        }
    }
}