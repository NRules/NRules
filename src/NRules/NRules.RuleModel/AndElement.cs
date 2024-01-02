using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Grouping element based on the logical AND condition.
/// </summary>
public class AndElement : GroupElement
{
    internal AndElement(IEnumerable<RuleElement> childElements)
        : base(childElements)
    {
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.And;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitAnd(context, this);
    }

    internal AndElement Update(IReadOnlyCollection<RuleElement> childElements)
    {
        if (ReferenceEquals(childElements, ChildElements)) return this;
        return new AndElement(childElements);
    }
}