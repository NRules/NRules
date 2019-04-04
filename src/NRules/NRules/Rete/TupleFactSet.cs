using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("TupleFactSet Tuple({Tuple.Count}) Facts({Facts.Count})")]
    internal class TupleFactSet
    {
        public Tuple Tuple { get; }
        public List<Fact> Facts { get; }

        public TupleFactSet(Tuple tuple, List<Fact> facts)
        {
            Tuple = tuple;
            Facts = facts;
        }
    }
}