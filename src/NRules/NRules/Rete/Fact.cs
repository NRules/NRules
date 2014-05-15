using System;
using System.Collections.Generic;

namespace NRules.Rete
{
    internal class Fact
    {
        public Fact(object @object)
        {
            Object = @object;
            FactType = @object.GetType();
            ChildTuples = new List<Tuple>();
        }

        public Type FactType { get; private set; }
        public object Object { get; private set; }
        public IList<Tuple> ChildTuples { get; private set; }
    }

    internal class WrapperFact : Fact
    {
        public WrapperFact(Tuple tuple)
            : base(tuple)
        {
        }

        public Tuple WrappedTuple { get { return (Tuple) Object; } }
    }
}