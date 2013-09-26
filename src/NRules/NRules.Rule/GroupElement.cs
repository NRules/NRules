using System.Collections.Generic;

namespace NRules.Rule
{
    internal enum GroupType
    {
        And = 0,
        Or = 1,
        Not = 2,
        Exists = 3,
    }

    public class GroupElement : RuleElement
    {
        private readonly List<RuleElement> _childElements = new List<RuleElement>(); 
        
        internal GroupElement(GroupType groupType)
        {
            GroupType = groupType;
        }

        internal GroupType GroupType { get; private set; }

        public IEnumerable<RuleElement> ChildElements
        {
            get { return _childElements; }
        }

        internal void AddChild(RuleElement ruleElement)
        {
            _childElements.Add(ruleElement);
            ruleElement.SymbolTable.ParentScope = SymbolTable;
        }
    }
}