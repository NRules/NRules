using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    [Name("Rule with metadata"), Description("Rule description")]
    [Tag("Test"), Tag("Metadata")]
    [Priority(100)]
    public class RuleWithMetadata : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}