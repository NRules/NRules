using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public abstract class BaseRulesTestFixture : RulesTestFixture
{
    protected BaseRulesTestFixture() : base(new XUnitRuleAsserter())
    {
        SetUpRules(Setup);
    }

    protected abstract void SetUpRules(IRepositorySetup setup);
}
