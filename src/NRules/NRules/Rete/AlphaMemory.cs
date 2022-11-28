using System.Collections.Generic;
using NRules.Utilities;

namespace NRules.Rete;

internal interface IAlphaMemory : ICanDeepClone<IAlphaMemory>
{
    IEnumerable<Fact> Facts { get; }
    int FactCount { get; }
    bool Contains(Fact fact);
    void Add(List<Fact> facts);
    void Remove(List<Fact> facts);
}

internal class AlphaMemory : IAlphaMemory
{
    private readonly HashSet<Fact> _facts = new();

    public IAlphaMemory DeepClone()
    {
        var memory = new AlphaMemory();
        _facts.CloneInto(memory._facts);
        return memory;
    }

    public IEnumerable<Fact> Facts => _facts;
    public int FactCount => _facts.Count;

    public bool Contains(Fact fact)
    {
        return _facts.Contains(fact);
    }

    public void Add(List<Fact> facts)
    {
        foreach (var fact in facts)
        {
            _facts.Add(fact);
        }
    }

    public void Remove(List<Fact> facts)
    {
        foreach (var fact in facts)
        {
            _facts.Remove(fact);
        }
    }
}
