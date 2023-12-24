using System.Collections.Generic;

namespace NRules.Rete;

internal interface IAlphaMemory
{
    IReadOnlyCollection<Fact> Facts { get; }
    bool Contains(Fact fact);
    void Add(List<Fact> facts);
    void Remove(List<Fact> facts);
}

internal class AlphaMemory : IAlphaMemory
{
    private readonly HashSet<Fact> _facts = new();

    public IReadOnlyCollection<Fact> Facts => _facts;

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
