using System.Collections.Generic;

namespace NRules.Rete;

internal interface IAlphaMemory
{
    IEnumerable<Fact> Facts { get; }
    int FactCount { get; }
    bool Contains(Fact fact);
    void Add(IEnumerable<Fact> facts);
    void Remove(IEnumerable<Fact> facts);
}

internal class AlphaMemory : IAlphaMemory
{
    private readonly HashSet<Fact> _facts = new();

    public IEnumerable<Fact> Facts => _facts;

    public int FactCount => _facts.Count;

    public bool Contains(Fact fact)
    {
        return _facts.Contains(fact);
    }

    public void Add(IEnumerable<Fact> facts)
    {
        foreach (var fact in facts)
        {
            _facts.Add(fact);
        }
    }

    public void Remove(IEnumerable<Fact> facts)
    {
        foreach (var fact in facts)
        {
            _facts.Remove(fact);
        }
    }
}
