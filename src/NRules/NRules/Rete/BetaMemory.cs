using System.Collections.Generic;
using NRules.Utilities;

namespace NRules.Rete;

internal interface IBetaMemory
{
    IReadOnlyCollection<Tuple> Tuples { get; }
    void Add(IEnumerable<Tuple> tuples);
    void Remove(IEnumerable<Tuple> tuples);
    Tuple? FindTuple(Tuple leftTuple, Fact? rightFact);
}

internal class BetaMemory : IBetaMemory
{
    private readonly HashSet<Tuple> _tuples = new();
    private readonly Dictionary<(Tuple Left, Fact? Right), Tuple> _parentToChildMap = new();

    public IReadOnlyCollection<Tuple> Tuples => _tuples;

    public void Add(IEnumerable<Tuple> tuples)
    {
        foreach (var tuple in tuples)
        {
            if (_tuples.Add(tuple))
            {
                AddMapping(tuple);
            }
        }
    }

    public void Remove(IEnumerable<Tuple> tuples)
    {
        foreach (var tuple in tuples)
        {
            if (_tuples.Remove(tuple))
                RemoveMapping(tuple);
        }
    }

    public Tuple? FindTuple(Tuple leftTuple, Fact? rightFact)
    {
        return _parentToChildMap.GetValueOrDefault((leftTuple, rightFact));
    }

    private void AddMapping(Tuple tuple)
    {
        if (tuple.Parent is not null)
            _parentToChildMap[(tuple.Parent, tuple.Fact)] = tuple;
    }

    private void RemoveMapping(Tuple tuple)
    {
        if (tuple.Parent is not null)
            _parentToChildMap.Remove((tuple.Parent, tuple.Fact));
    }
}