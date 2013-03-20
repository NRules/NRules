using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public class ConditionElement : RuleComponent
    {
        private readonly List<Condition> _conditions = new List<Condition>();

        internal ConditionElement(RuleComponentTypes componentType) : base(componentType)
        {
        }

        internal ConditionElement() : base(RuleComponentTypes.Conditional)
        {
        }

        public IEnumerable<Condition> Conditions
        {
            get { return _conditions; }
        }

        internal void Add(Condition condition)
        {
            _conditions.Add(condition);
        }
    }
}