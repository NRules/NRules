using NRules.Fluent;
using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public abstract class BaseRuleTestFixture
{
    private readonly RuleTestFixture _fixture;

    protected BaseRuleTestFixture(IRuleActivator activator = null, RuleCompiler compiler = null)
    {
        _fixture = new RuleTestFixture(new CachedRuleActivator(activator ?? new RuleRepository().Activator), compiler ?? new RuleCompiler());

        SetUpRules(_fixture.Setup);
    }

    protected RuleTestFixture Fixture => _fixture;

    protected ISession Session => _fixture.Session;

    protected abstract void SetUpRules(IRepositorySetup setup);
}
