using System.Collections.Generic;

namespace NRules.Rule
{
    public enum GroupType
    {
        And = 0,
        Or = 1,
        Not = 2,
        Exists = 3,
    }

    public class GroupElement : RuleElement
    {
        private readonly List<RuleElement> _childElements; 
        
        internal GroupElement(GroupType groupType, IEnumerable<RuleElement> childElements)
        {
            GroupType = groupType;
            _childElements = new List<RuleElement>(childElements);
        }

        internal GroupType GroupType { get; private set; }

        public IEnumerable<RuleElement> ChildElements
        {
            get { return _childElements; }
        }
    }
}