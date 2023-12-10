using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public abstract class BaseRulesTestFixture : RulesTestFixture
{
    protected BaseRulesTestFixture()
    {
        Asserter = new XUnitRuleAsserter();
        SetUpRules(Setup);
    }

    protected abstract void SetUpRules(IRulesTestSetup setup);
}
