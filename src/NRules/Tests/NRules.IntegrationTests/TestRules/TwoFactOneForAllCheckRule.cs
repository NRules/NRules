using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactOneForAllCheckRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .All<FactType2>(f => f.JoinProperty == fact1.TestProperty,
                    f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}