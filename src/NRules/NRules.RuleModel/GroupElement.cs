using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Type of group element.
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// Logical AND.
        /// </summary>
        And = 0,

        /// <summary>
        /// Logical OR.
        /// </summary>
        Or = 1,

        /// <summary>
        /// Logical NOT.
        /// </summary>
        Not = 2,

        /// <summary>
        /// Existential quantifier.
        /// </summary>
        Exists = 3,
    }

    /// <summary>
    /// Grouping element that logically combines the patterns.
    /// </summary>
    public class GroupElement : RuleElement
    {
        private readonly List<RuleElement> _childElements;

        internal GroupElement(GroupType groupType, IEnumerable<RuleElement> childElements)
        {
            GroupType = groupType;
            _childElements = new List<RuleElement>(childElements);
        }

        /// <summary>
        /// Type of grouping.
        /// </summary>
        public GroupType GroupType { get; private set; }

        /// <summary>
        /// List of child elements in the grouping.
        /// </summary>
        public IEnumerable<RuleElement> ChildElements
        {
            get { return _childElements; }
        }
    }
}