using NRules.Fluent;
using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public abstract class BaseRulesTestFixture : RulesTestFixture
{
    private static readonly IRuleActivator DefaultActivator = new RuleRepository().Activator;

    protected BaseRulesTestFixture(IRuleActivator activator = null, RuleCompiler compiler = null)
        : base((activator ?? DefaultActivator).AsCached(), compiler ?? new RuleCompiler(), new XUnitRuleAsserter())
    {
        SetUpRules(Setup);
    }

    protected abstract void SetUpRules(IRepositorySetup setup);
}
