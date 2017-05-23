using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("TupleFactSet Tuple({Tuple.Count}) Facts({Facts.Count})")]
    internal class TupleFactSet
    {
        public Tuple Tuple { get; }
        public IList<Fact> Facts { get; }

        public TupleFactSet(Tuple tuple, IList<Fact> facts)
        {
            Tuple = tuple;
            Facts = facts;
        }
    }
}