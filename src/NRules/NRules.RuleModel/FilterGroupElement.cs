using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that groups filters that determine which rule matches should trigger rule actions.
    /// </summary>
    public class FilterGroupElement : RuleElement
    {
        private readonly List<FilterElement> _filters;

        internal FilterGroupElement(IEnumerable<FilterElement> filters)
        {
            _filters = new List<FilterElement>(filters);

            AddImports(_filters);
        }

        /// <summary>
        /// List of filters the group element contains.
        /// </summary>
        public IEnumerable<FilterElement> Filters => _filters;

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitFilterGroup(context, this);
        }
    }
}