using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public sealed class XUnitRuleAsserter : IRuleAsserter
{
    public void Assert(RuleFireAssertResult result)
    {
        if (result.Expected != result.Actual)
        {
            throw new RuleFiredAssertionException(result.Expected, result.Actual, result.RuleMetadata.Name);
        }
    }
}
