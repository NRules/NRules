using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element that logically combines the patterns.
    /// </summary>
    public abstract class GroupElement : RuleLeftElement
    {
        private readonly List<RuleLeftElement> _childElements;

        internal GroupElement(IEnumerable<Declaration> declarations, IEnumerable<RuleLeftElement> childElements)
            : base(declarations)
        {
            _childElements = new List<RuleLeftElement>(childElements);
        }

        /// <summary>
        /// List of child elements in the grouping.
        /// </summary>
        public IEnumerable<RuleLeftElement> ChildElements => _childElements;
    }
}