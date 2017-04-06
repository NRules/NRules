using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("TupleFactSet Tuple({Tuple.Count}) Facts({Facts.Count})")]
    internal class TupleFactSet
    {
        public Tuple Tuple { get; private set; }
        public IList<Fact> Facts { get; private set; }

        public TupleFactSet(Tuple tuple, IList<Fact> facts)
        {
            Tuple = tuple;
            Facts = facts;
        }
    }
}