using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    [Tag("ParentTag"), Tag("ParentMetadata")]
    [Priority(200)]
    public abstract class ParentRuleWithMetadata : BaseRule
    {
    }
}
