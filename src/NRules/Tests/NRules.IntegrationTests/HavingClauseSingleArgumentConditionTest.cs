using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class HavingClauseSingleArgumentConditionTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_TwoMatchingFactsSameTypeHavingConditionOnFirstAttachedToSecond_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { Discriminator = "Type1", TestProperty = "Valid" };
        var fact2 = new FactType1 { Discriminator = "Type2", TestProperty = "Invalid" };

        Session.InsertAll(new[] { fact1, fact2 });

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string? Discriminator { get; set; }
        public string? TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1? fact1 = default;
            FactType1? fact2 = default;

            When()
                .Match(() => fact1, i => i!.Discriminator == "Type1")
                .Match(() => fact2, o => o!.Discriminator == "Type2")
                .Having(() => fact1!.TestProperty == "Valid");

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}