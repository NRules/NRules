using System;
using System.Diagnostics;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Rete
{
    [DebuggerDisplay("Fact {Object}")]
    internal class Fact : IFact
    {
        private readonly TypeInfo _factType;
        private object _object;

        public Fact()
        {
        }

        public Fact(object @object)
        {
            _object = @object;
            var factType = @object.GetType();
            _factType = factType.GetTypeInfo();
        }

        public virtual TypeInfo FactType
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

        public virtual bool IsWrapperFact
        {
            get { return false; }
        }

        Type IFact.Type
        {
            get { return FactType.AsType(); }
        }

        object IFact.Value
        {
            get { return Object; }
        }
    }

    [DebuggerDisplay("Wrapper Tuple({WrappedTuple.Count})")]
    internal class WrapperFact : Fact
    {
        public WrapperFact(Tuple tuple)
            : base(tuple)
        {
        }

        public override TypeInfo FactType
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

        public override bool IsWrapperFact
        {
            get { return true; }
        }
    }
}