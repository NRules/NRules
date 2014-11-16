using System;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("Fact {Object}")]
    internal class Fact
    {
        public Fact()
        {
        }

        public Fact(object @object)
        {
            Object = @object;
            FactType = @object.GetType();
        }

        public Type FactType { get; private set; }
        public object Object { get; private set; }
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