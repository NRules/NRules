using NRules.Fluent.Dsl;

namespace NRules.IntegrationTests.TestAssets
{
    [TagAttribute("ParentTag"), TagAttribute("ParentMetadata")]
    public abstract class BaseRuleWithMetadata : BaseRule
    {
    }
}
