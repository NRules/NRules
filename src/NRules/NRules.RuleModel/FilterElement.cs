using System.Linq.Expressions;

namespace NRules.RuleModel
{
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

        /// <summary>
        /// Type of rule match filter.
        /// </summary>
        public FilterType FilterType { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitFilter(context, this);
        }
    }
}