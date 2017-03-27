using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneEquatableFactRule : BaseRule
    {
        public override void Define()
        {
            EquatableFact fact1 = null;

            When()
                .Match<EquatableFact>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}