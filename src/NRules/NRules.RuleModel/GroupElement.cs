using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element that logically combines the patterns or other grouping elements.
    /// </summary>
    public abstract class GroupElement : RuleElement
    {
        private readonly List<RuleElement> _childElements;

        internal GroupElement(IEnumerable<RuleElement> childElements)
        {
            _childElements = new List<RuleElement>(childElements);

            AddExports(_childElements);
            AddImports(_childElements);
        }

        /// <summary>
        /// List of child elements in the grouping.
        /// </summary>
        public IEnumerable<RuleElement> ChildElements => _childElements;
    }
}