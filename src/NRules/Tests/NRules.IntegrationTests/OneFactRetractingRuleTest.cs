using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactRetractingRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFact_FiresOnceAndRetractsFact()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.Equal(0, Session.Query<FactType>().Count());
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType fact = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.Retract(fact));
        }
    }
}