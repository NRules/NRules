using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public class PatternElement : RuleElement
    {
        private readonly List<ConditionElement> _conditions;

        internal PatternElement(Type valueType, IEnumerable<ConditionElement> conditions)
        {
            ValueType = valueType;
            _conditions = new List<ConditionElement>(conditions);
        }

        internal PatternElement(Type valueType, IEnumerable<ConditionElement> conditions, PatternSourceElement source)
            : this(valueType, conditions)
        {
            Source = source;
        }

        public PatternSourceElement Source { get; private set; }
        public Type ValueType { get; private set; }

        public IEnumerable<ConditionElement> Conditions
        {
            get { return _conditions; }
        }
    }
}