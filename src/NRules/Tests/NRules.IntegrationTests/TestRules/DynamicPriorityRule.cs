using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class DynamicPriorityRule : BaseRule
    {
        public override void Define()
        {
            FactType5 fact5 = null;

            When()
                .Match<FactType5>(() => fact5, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action());

            Priority(() => fact5.TestCount);
        }
    }
}