using System;

namespace NRules.Rule
{
    public class AggregateElement : MatchElement
    {
        public Type AggregateType { get; private set; }

        internal AggregateElement(Type valueType, MatchElement source, Type aggregateType) : base(valueType, source)
        {
            AggregateType = aggregateType;
            RuleElementType = RuleElementTypes.Aggregate;
        }
    }
}