using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a group of rule match filters.
    /// </summary>
    public class FilterGroupBuilder : RuleElementBuilder, IBuilder<FilterGroupElement>
    {
        private readonly List<FilterElement> _filters = new List<FilterElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterGroupBuilder"/>.
        /// </summary>
        public FilterGroupBuilder()
        {
        }

        /// <summary>
        /// Adds a filter element to the group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Filter(FilterElement element)
        {
            _filters.Add(element);
        }

        /// <summary>
        /// Adds a filter to the group element.
        /// </summary>
        /// <param name="filterType">Type of filter.</param>
        /// <param name="expression">Filter expression.</param>
        public void Filter(FilterType filterType, LambdaExpression expression)
        {
            var filter = Element.Filter(filterType, expression);
            _filters.Add(filter);
        }

        FilterGroupElement IBuilder<FilterGroupElement>.Build()
        {
            var filterGroup = Element.FilterGroup(_filters);
            return filterGroup;
        }
    }
}