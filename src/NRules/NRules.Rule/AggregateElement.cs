using System;

namespace NRules.Rule
{
    public class AggregateElement : PatternSourceElement
    {
        public Type AggregateType { get; private set; }
        public PatternElement Source { get; private set; }

        internal AggregateElement(Type resultType, Type aggregateType, PatternElement source) : base(resultType)
        {
            AggregateType = aggregateType;
            Source = source;
            Source.SymbolTable.ParentScope = SymbolTable;
        }
    }
}