using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element that logically combines the patterns or other grouping elements.
    /// </summary>
    public abstract class GroupElement : RuleLeftElement
    {
        private readonly List<RuleLeftElement> _childElements;

        internal GroupElement(IEnumerable<RuleLeftElement> childElements)
        {
            _childElements = new List<RuleLeftElement>(childElements);

            AddExports(_childElements);
            AddImports(_childElements);
        }

        /// <summary>
        /// List of child elements in the grouping.
        /// </summary>
        public IEnumerable<RuleLeftElement> ChildElements => _childElements;
    }
}