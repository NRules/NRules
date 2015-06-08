using System;
using System.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("Fact {Object}")]
    internal class Fact
    {
        private readonly Type _factType;
        private object _object;

        public Fact()
        {
        }

        public Fact(object @object)
        {
            _object = @object;
            _factType = @object.GetType();
        }

        public virtual Type FactType
        {
            get { return _factType; }
        }

        public object RawObject
        {
            get { return _object; }
            set { _object = value; }
        }

        public virtual object Object
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

        public override Type FactType
        {
            get { return WrappedTuple.RightFact.FactType; }
        }

        public override object Object
        {
            get { return WrappedTuple.RightFact.Object; }
        }

        public Tuple WrappedTuple
        {
            get { return (Tuple) RawObject; }
        }
    }
}