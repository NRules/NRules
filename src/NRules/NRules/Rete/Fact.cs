using System;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("Fact {Object}")]
    internal class Fact
    {
        private readonly Type _factType;
        private readonly object _object;

        public Fact()
        {
        }

        public Fact(object @object)
        {
            _object = @object;
            _factType = @object.GetType();
        }

        public Type FactType
        {
            get { return _factType; }
        }

        public object Object
        {
            get { return _object; }
        }
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