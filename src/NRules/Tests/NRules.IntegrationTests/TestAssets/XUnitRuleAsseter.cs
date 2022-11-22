using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public sealed class XUnitRuleAsseter : IRuleAsseter
{
    public void Assert(IRuleFireAssertResult result)
    {
        if (result.Expected != result.Actual)
        {
            throw new RuleFiredAssertionException(result.Expected, result.Actual, result.Rule.Name);
        }
    }
}
