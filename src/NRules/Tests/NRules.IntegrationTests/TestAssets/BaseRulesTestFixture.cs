using NRules.Fluent;
using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public abstract class BaseRulesTestFixture : RulesTestFixture
{
    protected BaseRulesTestFixture(IRuleActivator activator = null, RuleCompiler compiler = null)
        : base(new CachedRuleActivator(activator ?? new RuleRepository().Activator), compiler ?? new RuleCompiler(), new XUnitRuleAsserter())
    {
        SetUpRules(Setup);
    }

    protected abstract void SetUpRules(IRepositorySetup setup);
}
