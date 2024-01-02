using NRules.RuleModel.Builders;

namespace NRules.RuleModel;

/// <summary>
/// Negative existential quantifier.
/// </summary>
public class NotElement : RuleElement
{
    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.Not;

    /// <summary>
    /// Fact source of the not element.
    /// </summary>
    public RuleElement Source { get; }

    internal NotElement(RuleElement source)
    {
        Source = source;

        AddImports(source);
    }

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitNot(context, this);
    }

    internal NotElement Update(RuleElement source)
    {
        if (ReferenceEquals(Source, source)) return this;
        return new NotElement(source);
    }
}