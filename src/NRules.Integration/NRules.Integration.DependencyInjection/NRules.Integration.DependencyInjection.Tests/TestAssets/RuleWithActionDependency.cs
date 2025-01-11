using NRules.Fluent.Dsl;

namespace NRules.Integration.DependencyInjection.Tests.TestAssets;

public class RuleWithActionDependency : Rule
{
    public override void Define()
    {
        TestFact1 fact1 = default!;

        When()
            .Match(() => fact1);

        Then()
            .Do(ctx => ctx.Resolve<ITestService>().DoIt());
    }
}
