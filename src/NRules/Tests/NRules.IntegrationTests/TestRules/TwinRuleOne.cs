using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwinRuleOne : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty);

            Then()
                .Do(ctx => Action());
        }
    }
}