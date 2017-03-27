using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    [Name("Declarative name")]
    [Priority(500)]
    public class RuleWithMetadataAndOverride : ParentRuleWithMetadata
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            Name("Programmatic name");
            Priority(1000);

            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}