using System.Linq.Expressions;

namespace NRules.RuleModel;

/// <summary>
/// Type of filter applied to rule matches.
/// </summary>
public enum FilterType
{
    /// <summary>
    /// Filter based on a predicate expression.
    /// </summary>
    Predicate,

    /// <summary>
    /// Filter that only accepts matches that result in a change of a given key.
    /// </summary>
    KeyChange,
}

/// <summary>
/// Filter that determines which rule matches should trigger rule actions.
/// </summary>
public class FilterElement : ExpressionElement
{
    internal FilterElement(FilterType filterType, LambdaExpression expression) 
        : base(expression)
    {
        FilterType = filterType;
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.Filter;

    /// <summary>
    /// Type of rule match filter.
    /// </summary>
    public FilterType FilterType { get; }

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitFilter(context, this);
    }
}