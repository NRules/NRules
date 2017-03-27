using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class IdentityMatchRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            FactType1 fact2 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match<FactType1>(() => fact2, f => ReferenceEquals(f, fact1));

            Then()
                .Do(ctx => Action(ctx));
        }
    }
}