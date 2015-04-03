using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwinRuleTwo : BaseRule
    {
        public override void Define()
        {
            FactType1 fact11 = null;
            FactType2 fact21 = null;

            When()
                .Match<FactType1>(() => fact11, f => f.TestProperty.StartsWith("Valid"))
                .Match<FactType2>(() => fact21, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact11.TestProperty);

            Then()
                .Do(ctx => Action());
        }
    }
}