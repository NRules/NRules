using System;

namespace NRules.Testing;

internal class DefaultRuleAsserter : IRuleAsserter
{
    public void Assert(RuleAssertResult result)
    {
        if (result.Status != RuleAssertStatus.Passed)
            throw new Exception(result.GetMessage());
    }
}