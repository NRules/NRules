using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneGroupByFlattenRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_NoMatchingFacts_DoesNotFire()
    {
        //Arrange - Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_TwoFactsForOneGroup_FiresTwiceWithFactsFromGroupOne()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupInsertedThenOneUpdated_FiresTwiceWithFactsFromGroupOne()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresTwiceWithFactsFromGroupOne()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };
        var fact3 = new FactType { TestProperty = "Valid Value Group2" };

        var facts = new[] { fact1, fact2, fact3 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresWithEachFactFromEachGroup()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };
        var fact3 = new FactType { TestProperty = "Valid Value Group2" };
        var fact4 = new FactType { TestProperty = "Valid Value Group2" };

        var facts = new[] { fact1, fact2, fact3, fact4 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
            s.Rule().Fired(Matched.Fact(fact3));
            s.Rule().Fired(Matched.Fact(fact4));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresTwiceWithFactsFromGroupOne()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };
        var fact3 = new FactType { TestProperty = "Valid Value Group2" };
        var fact4 = new FactType { TestProperty = "Valid Value Group2" };

        var facts = new[] { fact1, fact2, fact3, fact4 };
        Session.InsertAll(facts);

        Session.Retract(fact4);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresTwiceWithFactsFromGroupOne()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };
        var fact3 = new FactType { TestProperty = "Valid Value Group2" };
        var fact4 = new FactType { TestProperty = "Valid Value Group2" };

        var facts = new[] { fact1, fact2, fact3, fact4 };
        Session.InsertAll(facts);

        fact4.TestProperty = "Invalid Value";
        Session.Update(fact4);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresThreeTimesWithFactsFromGroupOne()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };
        var fact3 = new FactType { TestProperty = "Valid Value Group2" };
        var fact4 = new FactType { TestProperty = "Valid Value Group2" };

        var facts = new[] { fact1, fact2, fact3, fact4 };
        Session.InsertAll(facts);

        fact4.TestProperty = "Valid Value Group1";
        Session.Update(fact4);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
            s.Rule().Fired(Matched.Fact(fact4));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresWithEachFactFromEachGroup()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };
        var fact3 = new FactType { TestProperty = "Valid Value Group2" };
        var fact4 = new FactType { TestProperty = "Invalid Value" };

        var facts = new[] { fact1, fact2, fact3, fact4 };
        Session.InsertAll(facts);

        fact4.TestProperty = "Valid Value Group2";
        Session.Update(fact4);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
            s.Rule().Fired(Matched.Fact(fact3));
            s.Rule().Fired(Matched.Fact(fact4));
        });
    }

    [Fact]
    public void Fire_TwoFactsForOneGroupAssertedThenOneRetractedAnotherUpdatedThenOneAssertedBack_FiresTwiceWithFactsFromOneGroup()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value Group1" };
        var fact2 = new FactType { TestProperty = "Valid Value Group1" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        Session.Retract(fact2);

        Session.Update(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact(fact1));
            s.Rule().Fired(Matched.Fact(fact2));
        });
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
                .Query(() => fact, q => q
                    .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.TestProperty)
                    .Where(g => g.Count() > 1)
                    .SelectMany(x => x));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}