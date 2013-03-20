using System;

namespace NRules.Rule
{
    public class AggregateElement : ConditionElement
    {
        internal AggregateElement() : base(RuleComponentTypes.Aggregate)
        {
        }

        public Declaration Declaration { get; internal set; }
    }
}