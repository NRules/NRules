using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public sealed class XUnitRuleAsserter : IRuleAsserter
{
    public void Assert(RuleAssertResult result)
    {
        if (result.Status == RuleAssertStatus.Failed)
        {
            throw new RuleFiredAssertionException(result.GetMessage());
        }
    }
}
