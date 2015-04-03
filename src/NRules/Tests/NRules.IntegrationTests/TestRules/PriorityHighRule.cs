using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    [Priority(100)]
    public class PriorityHighRule : BaseRule
    {
        public override void Define()
        {
            FactType2 fact2 = null;

            When()
                .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action());
        }
    }
}