using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete;

[DebuggerDisplay("TupleFactSet Tuple({Tuple.Count}) Facts({Facts.Count})")]
internal class TupleFactSet(Tuple tuple, List<Fact> facts)
{
    public Tuple Tuple { get; } = tuple;
    public List<Fact> Facts { get; } = facts;
}