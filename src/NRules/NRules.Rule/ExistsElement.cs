using System;

namespace NRules.Rule
{
    public class ExistsElement : ConditionElement
    {
        internal ExistsElement() : base(RuleComponentTypes.Existential)
        {
        }

        public Declaration Declaration { get; internal set; }
    }
}