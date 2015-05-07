using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    [Priority(500)]
    public class RuleWithMetadataAndPriorityOverride : ParentRuleWithMetadata
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            Priority(1000);

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action());
        }
    }
}