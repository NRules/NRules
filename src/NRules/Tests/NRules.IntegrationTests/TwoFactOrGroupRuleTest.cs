using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOrGroupRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_NoMatchingFacts_DoesNotFire()
    {
        //Arrange
        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_FactMatchingFirstPartOfOrGroup_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
    }

    [Fact]
    public void Fire_FactsMatchingSecondPartOfOrGroup_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
    }

    [Fact]
    public void Fire_FactsMatchingBothPartsOfOrGroup_FiresTwice()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact12.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(2);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string? TestProperty { get; set; }
    }

    public class FactType2
    {
        public string? TestProperty { get; set; }
        public string? JoinProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1? fact1 = null;
            FactType2? fact2 = null;

            When()
                .Or(x => x
                    .Match(() => fact1, f => f!.TestProperty!.StartsWith("Valid"))
                    .And(xx => xx
                        .Match(() => fact1, f => f!.TestProperty!.StartsWith("Invalid"))
                        .Match(() => fact2, f => f!.TestProperty!.StartsWith("Valid"), f => f!.JoinProperty == fact1!.TestProperty)));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}