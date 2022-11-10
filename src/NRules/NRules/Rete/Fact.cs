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

    public virtual Type? FactType { get; }

    public object? RawObject { get; set; }

    public virtual IFactSource? Source
    {
        get => null;
        set => throw new InvalidOperationException("Source is only supported on synthetic facts");
    }

    public virtual object? Object => RawObject;
    public virtual bool IsWrapperFact => false;
    Type? IFact.Type => FactType;
    object? IFact.Value => Object;
    IFactSource? IFact.Source => Source;
}

[DebuggerDisplay("Fact {Source.SourceType} {Object}")]
internal sealed class SyntheticFact : Fact
{
    public SyntheticFact(object @object)
        : base(@object)
    {
    }

    public override IFactSource? Source { get; set; }
}

[DebuggerDisplay("Fact Tuple({WrappedTuple.Count}) -> {Object}")]
internal sealed class WrapperFact : Fact
{
    public WrapperFact(Tuple tuple)
        : base(tuple)
    {
    }

    public override Type? FactType => RightFact?.FactType;
    public override object? Object => RightFact?.Object;
    public Tuple WrappedTuple => (Tuple)RawObject!;
    public override bool IsWrapperFact => true;

    public override IFactSource? Source
    {
        get => RightFact?.Source;
        set { }
    }

    private Fact? RightFact => WrappedTuple.RightFact;
}