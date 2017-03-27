using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class OneFactNoBindingRule : BaseRule
    {
        public override void Define()
        {
            When()
                .Match<FactType1>(f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}