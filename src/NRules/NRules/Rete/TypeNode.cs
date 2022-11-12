using System.Diagnostics;
using NRules.Diagnostics;

namespace NRules.Rete;

[DebuggerDisplay("Type {FilterType.FullName,nq}")]
internal class TypeNode : AlphaNode
{
    public TypeNode(int id, Type filterType)
        : base(id, filterType)
    {
        FilterType = filterType;
    }

    public Type FilterType { get; }

    protected override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
    {
        var isMatchingType = FilterType.IsAssignableFrom(fact.FactType);
        return isMatchingType;
    }

    public override void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var toUpdate = new List<Fact>();
        using (var counter = PerfCounter.Update(context, this))
        {
            foreach (var fact in facts)
            {
                if (IsSatisfiedBy(context, fact))
                    toUpdate.Add(fact);
            }
            counter.AddInputs(facts.Count);
            counter.AddOutputs(toUpdate.Count);
        }

        PropagateUpdateInternal(context, toUpdate);
    }

    public override void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var toRetract = new List<Fact>();
        using (var counter = PerfCounter.Retract(context, this))
        {
            foreach (var fact in facts)
            {
                if (IsSatisfiedBy(context, fact))
                    toRetract.Add(fact);
            }
            counter.AddInputs(facts.Count);
            counter.AddOutputs(toRetract.Count);
        }

        PropagateRetractInternal(context, toRetract);
    }

    public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitTypeNode(context, this);
    }
}