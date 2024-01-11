using NRules.RuleModel.Builders;

namespace NRules.RuleModel;

/// <summary>
/// Existential quantifier.
/// </summary>
public class ExistsElement : RuleElement
{
    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.Exists;

    /// <summary>
    /// Fact source of the existential element.
    /// </summary>
    public RuleElement Source { get; }

    internal ExistsElement(RuleElement source)
    {
        Source = source;

        AddImports(source);
    }

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitExists(context, this);
    }

    internal ExistsElement Update(RuleElement source)
    {
        if (ReferenceEquals(Source, source)) return this;
        return new ExistsElement(source);
    }
}