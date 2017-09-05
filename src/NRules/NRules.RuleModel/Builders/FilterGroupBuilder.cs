using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a group of rule match filters.
    /// </summary>
    public class FilterGroupBuilder : RuleElementBuilder, IBuilder<FilterGroupElement>
    {
        private readonly List<FilterElement> _filters = new List<FilterElement>();

        internal FilterGroupBuilder(SymbolTable scope)
            : base(scope)
        {
        }

        /// <summary>
        /// Adds a filter to the group.
        /// </summary>
        /// <param name="filterType">Type of filter.</param>
        /// <param name="expression">Filter expression.</param>
        public void Filter(FilterType filterType, LambdaExpression expression)
        {
            IEnumerable<Declaration> references = expression.Parameters.Select<ParameterExpression, Declaration>(p => Scope.Lookup(p.Name, p.Type));
            var filter = new FilterElement(filterType, Scope.VisibleDeclarations, references, expression);
            _filters.Add(filter);
        }

        FilterGroupElement IBuilder<FilterGroupElement>.Build()
        {
            var filterGroup = new FilterGroupElement(Scope.VisibleDeclarations, _filters);
            return filterGroup;
        }
    }
}