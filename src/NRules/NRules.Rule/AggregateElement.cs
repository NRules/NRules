using System;

namespace NRules.Rule
{
    public class AggregateElement : PatternSourceElement
    {
        public Declaration Declaration { get; private set; }
        public Type AggregateType { get; private set; }
        public PatternElement Source { get; private set; }

        internal AggregateElement(Declaration declaration, Type aggregateType, PatternElement source) : base(declaration.Type)
        {
            Declaration = declaration;
            AggregateType = aggregateType;
            Source = source;
        }
    }
}