using System;
using System.Diagnostics;
using NRules.RuleModel;

namespace NRules.Rete;

[DebuggerDisplay("Fact {Object}")]
internal class Fact : IFact
{
    public Fact(object @object)
    {
        RawObject = @object;
        FactType = @object.GetType();
    }

    public Fact(object? @object, Type declaredType)
    {
        RawObject = @object;
        FactType = @object?.GetType() ?? declaredType;
    }

    public virtual Type FactType { get; }

    public object? RawObject { get; set; }

    public virtual IFactSource? Source
    {
        get => null;
        set => throw new InvalidOperationException("Source is only supported on synthetic facts");
    }

    public virtual object? Object => RawObject;
    public virtual bool IsWrapperFact => false;
    Type IFact.Type => FactType;
    object? IFact.Value => Object;
    IFactSource? IFact.Source => Source;
}

[DebuggerDisplay("Fact {Source.SourceType} {Object}")]
internal class SyntheticFact(object @object) : Fact(@object)
{
    public override IFactSource? Source { get; set; }
}

[DebuggerDisplay("Fact Tuple({WrappedTuple.Count}) -> {Object}")]
internal class WrapperFact(Tuple tuple) : Fact(tuple)
{
    public override Type FactType => WrappedTuple.RightFact?.FactType ?? typeof(void);
    public override object? Object => WrappedTuple.RightFact?.Object;
    public Tuple WrappedTuple => (Tuple) RawObject!;
    public override bool IsWrapperFact => true;

    public override IFactSource? Source => WrappedTuple.RightFact?.Source;
}