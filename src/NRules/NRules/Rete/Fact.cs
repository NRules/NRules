using System;
using System.Diagnostics;
using NRules.RuleModel;

namespace NRules.Rete
{
    [DebuggerDisplay("Fact {Object}")]
    internal class Fact : IFact
    {
        private object _object;

        public Fact()
        {
        }

        public Fact(object @object)
        {
            _object = @object;
            FactType = @object.GetType();
        }

        public Fact(object @object, Type declaredType)
        {
            _object = @object;
            FactType = @object?.GetType() ?? declaredType;
        }

        public virtual Type FactType { get; }

        public object RawObject
        {
            get => _object;
            set => _object = value;
        }

        public virtual IFactSource Source
        {
            get => null;
            set => throw new InvalidOperationException("Source is only supported on synthetic facts");
        }

        public virtual object Object => _object;
        public virtual bool IsWrapperFact => false;
        Type IFact.Type => FactType;
        object IFact.Value => Object;
        IFactSource IFact.Source => Source;
    }

    [DebuggerDisplay("Fact {Source.SourceType} {Object}")]
    internal class SyntheticFact : Fact
    {
        private IFactSource _source;

        public SyntheticFact(object @object)
            : base(@object)
        {
        }

        public override IFactSource Source
        {
            get => _source;
            set => _source = value;
        }
    }

    [DebuggerDisplay("Fact Tuple({WrappedTuple.Count}) -> {Object}")]
    internal class WrapperFact : Fact
    {
        public WrapperFact(Tuple tuple)
            : base(tuple)
        {
        }

        public override Type FactType => WrappedTuple.RightFact?.FactType;
        public override object Object => WrappedTuple.RightFact?.Object;
        public Tuple WrappedTuple => (Tuple) RawObject;
        public override bool IsWrapperFact => true;

        public override IFactSource Source
        {
            get => WrappedTuple.RightFact?.Source;
            set {}
        }
    }
}