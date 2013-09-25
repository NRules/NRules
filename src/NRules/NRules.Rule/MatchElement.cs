using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public class MatchElement : RuleElement
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();

        internal MatchElement(Type valueType)
        {
            RuleElementType = RuleElementTypes.Match;
            ValueType = valueType;
        }

        internal MatchElement(Type valueType, MatchElement source) : this(valueType)
        {
            Source = source;
            Source.SymbolTable.ParentScope = SymbolTable;
        }

        public MatchElement Source { get; private set; }
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