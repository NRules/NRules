using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete;

[DebuggerDisplay("TupleFactSet Tuple({Tuple.Count}) Facts({Facts.Count})")]
internal class TupleFactSet : IReadOnlyCollection<Fact>
{
    private readonly IReadOnlyCollection<Fact> _facts;

    public TupleFactSet(Tuple tuple, IReadOnlyCollection<Fact> facts)
    {
        Tuple = tuple;
        _facts = facts;
    }

    public Tuple Tuple { get; }

    public IReadOnlyCollection<Fact> Facts => _facts;

    public int Count => _facts.Count;

    public IEnumerator<Fact> GetEnumerator()
    {
        return _facts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_facts).GetEnumerator();
    }
}