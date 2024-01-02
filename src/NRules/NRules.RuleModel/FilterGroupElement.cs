using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups filters that determine which rule matches should trigger rule actions.
/// </summary>
public class FilterGroupElement : RuleElement
{
    private readonly FilterElement[] _filters;

    internal FilterGroupElement(IEnumerable<FilterElement> filters)
    {
        _filters = filters.ToArray();

        AddImports(_filters);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.FilterGroup;

    /// <summary>
    /// List of filters the group element contains.
    /// </summary>
    public IReadOnlyCollection<FilterElement> Filters => _filters;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitFilterGroup(context, this);
    }

    internal FilterGroupElement Update(IReadOnlyCollection<FilterElement> filters)
    {
        if (ReferenceEquals(filters, _filters)) return this;
        return new FilterGroupElement(filters);
    }
}