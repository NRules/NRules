using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Grouping element that logically combines the patterns.
    /// </summary>
    public abstract class GroupElement : RuleLeftElement
    {
        private readonly List<Declaration> _declarations;
        private readonly List<RuleLeftElement> _childElements;

        internal GroupElement(IEnumerable<Declaration> declarations, IEnumerable<RuleLeftElement> childElements)
        {
            _declarations = new List<Declaration>(declarations);
            _childElements = new List<RuleLeftElement>(childElements);
        }

        public override IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        /// <summary>
        /// List of child elements in the grouping.
        /// </summary>
        public IEnumerable<RuleLeftElement> ChildElements
        {
            get { return _childElements; }
        }
    }
}