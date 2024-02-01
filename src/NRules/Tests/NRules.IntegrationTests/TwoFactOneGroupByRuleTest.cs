using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOneGroupByRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnother_FiresTwiceWithOneFactInEachGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };
        var fact4 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1)));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndNoneOfAnother_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherInsertedInOppositeOrder_FiresTwiceWithOneFactInEachGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };
        var fact4 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 22" };

        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);
        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1)));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherThenFireThenAnotherMatchingFactForSecondGroupThenFire_FiresTwiceWithOneFactInEachGroupThenFiresAgainWithTwoFactsInOneGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act
        Session.Fire();
        
        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1)));

        Recorder.Clear();

        //Act
        Session.Insert(fact23);
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 2)));
    }

    [Fact]
    public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenOneRetracted_FiresOnceWithOneFactInGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Insert(fact3);

        Session.Retract(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1)));
    }

    [Fact]
    public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Insert(fact3);

        Session.Retract(fact2);
        Session.Retract(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FactOfOneKindIsInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21" };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FactOfOneKindIsAssertedThenRetractedAndTwoOfAnotherKindAreValid_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21" };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);

        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FactOfOneKindIsAssertedThenUpdatedToInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21" };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);

        fact1.TestProperty = "Invalid Value 1";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FactOfOneKindIsInvalidThenUpdatedToValidAndTwoOfAnotherKindAreValid_FiresTwiceWithOneFactInEachGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21" };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);

        fact1.TestProperty = "Valid Value 1";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1)));
    }

    [Fact]
    public void Fire_BulkInsertForMultipleTypes_FiresFourTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };
        var fact24 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        var facts = new object[] { fact11, fact12, fact21, fact22, fact23, fact24 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Exactly(4), Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 2)));
    }

    [Fact]
    public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFactsInsertInReverse_FiresFourTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };
        var fact24 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22" };

        var facts = new[] { fact24, fact23, fact22, fact21 };
        Session.InsertAll(facts);
        var facts2 = new[] { fact12, fact11 };
        Session.InsertAll(facts2);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Exactly(4), Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 2)));
    }

    [Fact]
    public void Fire_TwoMatchingCombinationsThenOneFactOfFirstKindUpdated_FiresTwiceBeforeUpdateAndOnceAfter()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };

        Session.Insert(fact11);
        Session.Insert(fact12);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify(x => x.Rule().Fired(Times.Twice));

        //Act - 2
        Session.Update(fact11);
        Session.Fire();

        //Assert - 2
        Verify(x => x.Rule().Fired(Times.Exactly(3)));
    }

    [Fact]
    public void Fire_TwoMatchingCombinationsThenOneFactOfSecondKindUpdated_FiresTwiceBeforeUpdateAndTwiceAfter()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21" };

        Session.Insert(fact11);
        Session.Insert(fact12);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify(x => x.Rule().Fired(Times.Twice));

        //Act - 2
        Session.Update(fact21);
        Session.Fire();

        //Assert - 2
        Verify(x => x.Rule().Fired(Times.Exactly(4)));
    }

    [Fact]
    public void Fire_OneFactOfOneKindAndAggregatedFactsMatchThenAggregatedFactUpdatedWithDifferentGroupKey_FiresTwiceFactsInCorrectGroups()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1" };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1" };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 2" };

        Session.Insert(fact11);
        Session.Insert(fact21);
        Session.Insert(fact22);
        Session.Insert(fact23);

        fact22.GroupKey = "Group 2";
        Session.Update(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 1 && g.Key == "Group 1"));
            s.Rule().Fired(Matched.Fact<IGrouping<string, FactType2>>(g => g.Count() == 2 && g.Key == "Group 2"));
        });
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class FactType2
    {
        [NotNull]
        public string? GroupKey { get; set; }
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact = null!;
            IGrouping<string, FactType2> group = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group, x => x
                    .Match<FactType2>(
                        f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.GroupKey));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}
