using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Grouping element based on the logical OR condition.
/// </summary>
public class OrElement : GroupElement
{
    internal OrElement(RuleElement[] childElements)
        : base(childElements)
    {
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.Or;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitOr(context, this);
    }

    internal OrElement Update(IReadOnlyList<RuleElement> childElements)
    {
        if (ReferenceEquals(childElements, ChildElements)) return this;
        return new OrElement(childElements.AsArray());
    }
}