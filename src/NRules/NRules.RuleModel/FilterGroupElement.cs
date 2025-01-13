using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups filters that determine which rule matches should trigger rule actions.
/// </summary>
public class FilterGroupElement : RuleElement
{
    private readonly FilterElement[] _filters;

    internal FilterGroupElement(FilterElement[] filters)
    {
        _filters = filters;

        AddImports(_filters);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.FilterGroup;

    /// <summary>
    /// List of filters the group element contains.
    /// </summary>
    public IReadOnlyList<FilterElement> Filters => _filters;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitFilterGroup(context, this);
    }

    internal FilterGroupElement Update(IReadOnlyList<FilterElement> filters)
    {
        if (ReferenceEquals(filters, _filters)) return this;
        return new FilterGroupElement(filters.AsArray());
    }
}