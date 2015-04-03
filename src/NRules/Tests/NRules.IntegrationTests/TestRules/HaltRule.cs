using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class HaltRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action())
                .Do(ctx => ctx.Halt());
        }
    }
}