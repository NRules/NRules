using System.Diagnostics;

namespace NRules.Rete;

[DebuggerDisplay("TupleFactSet Tuple({Tuple.Count}) Facts({Facts.Count})")]
internal class TupleFactSet
{
    public Tuple Tuple { get; }
    public IReadOnlyCollection<Fact> Facts { get; }

    public TupleFactSet(Tuple tuple, IEnumerable<Fact>? facts = null)
    {
        Tuple = tuple;
        Facts = facts?.ToArray() ?? Array.Empty<Fact>();
    }
}