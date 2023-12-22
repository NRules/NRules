using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class HaltRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_TwoMatchingFacts_FiresOnceAndHalts()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var fact2 = new FactType { TestProperty = "Valid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_TwoMatchingFactsFireCalledTwice_FiresOnceThenHaltsThenResumesAndFiresAgain()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var fact2 = new FactType { TestProperty = "Valid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        public string TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType fact = null;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.Halt());
        }
    }
}