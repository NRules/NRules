using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class TwoFactSameTypeRule : BaseRule
    {
        public override void Define()
        {
            FactType4 fact1 = null;
            FactType4 fact2 = null;

            When()
                .Match<FactType4>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match<FactType4>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.Parent == fact1);

            Then()
                .Do(ctx => Action(ctx));
        }
    }
}
