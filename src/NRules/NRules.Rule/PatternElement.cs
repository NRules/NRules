using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public class PatternElement : RuleElement
    {
        private readonly List<ConditionElement> _conditions;

        internal PatternElement(Declaration declaration, IEnumerable<ConditionElement> conditions)
        {
            Declaration = declaration;
            ValueType = declaration.Type;
            _conditions = new List<ConditionElement>(conditions);
        }

        internal PatternElement(Declaration declaration, IEnumerable<ConditionElement> conditions, PatternSourceElement source)
            : this(declaration, conditions)
        {
            Source = source;
        }

        public Declaration Declaration { get; private set; }
        public PatternSourceElement Source { get; private set; }
        public Type ValueType { get; private set; }

        public IEnumerable<ConditionElement> Conditions
        {
            get { return _conditions; }
        }
    }
}