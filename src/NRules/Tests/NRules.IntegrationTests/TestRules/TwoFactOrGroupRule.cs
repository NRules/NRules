using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactOrGroupRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType2 fact2 = null;

            When()
                .Or(x => x
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .And(xx => xx
                        .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Invalid"))
                        .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)));

            Then()
                .Do(ctx => Action());
        }
    }
}