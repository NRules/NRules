using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public class PatternElement : RuleElement
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();

        internal PatternElement(Type valueType)
        {
            ValueType = valueType;
        }

        internal PatternElement(Type valueType, PatternSourceElement source) : this(valueType)
        {
            Source = source;
            Source.SymbolTable.ParentScope = SymbolTable;
        }

        public PatternSourceElement Source { get; private set; }
        public Declaration Declaration { get; internal set; }
        public Type ValueType { get; private set; }

        public Declaration Declare(string name)
        {
            var declaration = new Declaration(name, ValueType, this);
            SymbolTable.Add(declaration);
            return declaration;
        }

        public IEnumerable<ConditionElement> Conditions
        {
            get { return _conditions; }
        }

        internal void Add(ConditionElement condition)
        {
            _conditions.Add(condition);
        }
    }
}