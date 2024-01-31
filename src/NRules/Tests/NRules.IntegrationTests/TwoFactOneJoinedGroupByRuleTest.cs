using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOneJoinedGroupByRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnother_FiresTwiceWithOneFactInEachGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = null };
        var fact4 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };
        var fact5 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 23", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4, fact5 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 1)));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndNoneOfAnother_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value", GroupKey = "Group 11" };

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
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = null };
        var fact4 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };
        var fact5 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 23", JoinProperty = fact1.TestProperty };

        var facts = new[] { fact2, fact3, fact4, fact5 };
        Session.InsertAll(facts);
        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 1)));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherThenFireThenAnotherMatchingFactForSecondGroupThenFire_FiresTwiceWithOneFactInEachGroupThenFiresAgainWithTwoFactsInOneGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 1)));

        //Act
        Recorder.Clear();
        Session.Insert(fact23);
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 2)));
    }

    [Fact]
    public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenOneRetracted_FiresOnceWithOneFactInGroup()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Insert(fact3);

        Session.Retract(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 1)));
    }

    [Fact]
    public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };

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
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };

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
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };

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
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty };

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
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1", GroupKey = "Group 11" };
        var fact2 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = "Valid Value 1" };
        var fact3 = new FactType2 { TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = "Valid Value 1" };
        var fact4 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = "Valid Value 1" };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4 };
        Session.InsertAll(facts);

        fact1.TestProperty = "Valid Value 1";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Times.Twice, Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 1)));
    }

    [Fact]
    public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingOneOfTheFacts_FiresOnceWithTwoFactsInGroups()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", GroupKey = "Group 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(s => s.Rule().Fired(Matched.Fact<IGrouping<string, GroupElement>>(f => f.Count() == 2)));
    }

    [Fact]
    public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFacts_FiresThreeTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", GroupKey = "Group 12" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact11.TestProperty };
        var fact24 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        var facts = new[] { fact21, fact22, fact23, fact24 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        FactType1? firedFact1 = null;
        FactType1? firedFact2 = null;
        FactType1? firedFact3 = null;
        IGrouping<string, GroupElement>? firedGroup1 = null;
        IGrouping<string, GroupElement>? firedGroup2 = null;
        IGrouping<string, GroupElement>? firedGroup3 = null;

        VerifySequence(s =>
        {
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact1 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup1 = f));
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact2 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup2 = f));
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact3 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup3 = f));
        });

        Assert.Equal(fact11, firedFact1);
        Assert.Equal(fact11, firedFact2);
        Assert.Equal(fact12, firedFact3);
        Assert.NotNull(firedGroup1);
        Assert.Equal(2, firedGroup1.Count());
        Assert.NotNull(firedGroup2);
        Assert.Single(firedGroup2);
        Assert.NotNull(firedGroup3);
        Assert.Single(firedGroup3);
    }

    [Fact]
    public void Fire_BulkInsertForMultipleTypes_FiresThreeTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", GroupKey = "Group 12" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact11.TestProperty };
        var fact24 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact12.TestProperty };

        var facts = new object[] { fact11, fact12, fact21, fact22, fact23, fact24 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        FactType1? firedFact1 = null;
        FactType1? firedFact2 = null;
        FactType1? firedFact3 = null;
        IGrouping<string, GroupElement>? firedGroup1 = null;
        IGrouping<string, GroupElement>? firedGroup2 = null;
        IGrouping<string, GroupElement>? firedGroup3 = null;

        VerifySequence(s =>
        {
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact1 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup1 = f));
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact2 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup2 = f));
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact3 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup3 = f));
        });

        Assert.Equal(fact11, firedFact1);
        Assert.Equal(fact11, firedFact2);
        Assert.Equal(fact12, firedFact3);
        Assert.NotNull(firedGroup1);
        Assert.Equal(2, firedGroup1.Count());
        Assert.NotNull(firedGroup2);
        Assert.Single(firedGroup2);
        Assert.NotNull(firedGroup3);
        Assert.Single(firedGroup3);
    }

    [Fact]
    public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFactsInsertInReverse_FiresThreeTimesWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", GroupKey = "Group 11" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact11.TestProperty };
        var fact24 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty };

        var facts = new[] { fact24, fact23, fact22, fact21 };
        Session.InsertAll(facts);
        var facts2 = new[] { fact12, fact11 };
        Session.InsertAll(facts2);

        //Act
        Session.Fire();

        //Assert
        FactType1? firedFact1 = null;
        FactType1? firedFact2 = null;
        FactType1? firedFact3 = null;
        IGrouping<string, GroupElement>? firedGroup1 = null;
        IGrouping<string, GroupElement>? firedGroup2 = null;
        IGrouping<string, GroupElement>? firedGroup3 = null;

        VerifySequence(s =>
        {
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact1 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup1 = f));
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact2 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup2 = f));
            s.Rule().Fired(
                Matched.Fact<FactType1>().Callback(f => firedFact3 = f),
                Matched.Fact<IGrouping<string, GroupElement>>().Callback(f => firedGroup3 = f));
        });

        Assert.Equal(fact12, firedFact1);
        Assert.Equal(fact11, firedFact2);
        Assert.Equal(fact11, firedFact3);
        Assert.NotNull(firedGroup1);
        Assert.Single(firedGroup1);
        Assert.NotNull(firedGroup2);
        Assert.Single(firedGroup2);
        Assert.NotNull(firedGroup3);
        Assert.Equal(2, firedGroup3.Count());
    }

    [Fact]
    public void Fire_TwoMatchingCombinationsThenOneFactOfFirstKindUpdated_FiresTwiceBeforeUpdateAndOnceAfter()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", GroupKey = "Group 12" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty };

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
    public void Fire_TwoMatchingCombinationsThenOneFactOfSecondKindUpdated_FiresTwiceBeforeUpdateAndOnceAfter()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 11" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", GroupKey = "Group 12" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty };

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
        Verify(x => x.Rule().Fired(Times.Exactly(3)));
    }

    [Fact]
    public void Fire_OneFactOfOneKindAndAggregatedFactsMatchThenFactUpdatedWithDifferentGroupKey_FiresOnceFactsInNewGroup()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact21);
        Session.Insert(fact22);

        fact11.GroupKey = "Group 2";
        Session.Update(fact11);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, GroupElement>>()
            .Callback(firedFact => Assert.Equal("Group 2|Group 1", firedFact.Key))));
    }

    [Fact]
    public void Fire_OneFactOfOneKindAndAggregatedFactsMatchThenAggregatedFactUpdatedWithDifferentGroupKey_FiresTwiceFactsInCorrectGroups()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 2", JoinProperty = fact11.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact21);
        Session.Insert(fact22);
        Session.Insert(fact23);

        fact22.GroupKey = "Group 2";
        Session.Update(fact22);

        //Act
        Session.Fire();

        //Assert
        VerifySequence(s =>
        {
            s.Rule().Fired(Matched.Fact<IGrouping<string, GroupElement>>()
                .Callback(firedFact =>
                {
                    Assert.Single(firedFact);
                    Assert.Equal("Group 1|Group 1", firedFact.Key);
                }));
            s.Rule().Fired(Matched.Fact<IGrouping<string, GroupElement>>()
                .Callback(firedFact =>
                {
                    Assert.Equal(2, firedFact.Count());
                    Assert.Equal("Group 1|Group 2", firedFact.Key);
                }));
        });
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        [NotNull]
        public string? GroupKey { get; set; }
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class FactType2
    {
        [NotNull]
        public string? GroupKey { get; set; }
        [NotNull]
        public string? TestProperty { get; set; }
        public string? JoinProperty { get; set; }
    }

    public class GroupElement
    {
        public GroupElement(FactType1 fact1, FactType2 fact2)
        {
            TestProperty = $"{fact1.TestProperty}|{fact2.TestProperty}";
        }

        public string TestProperty { get; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact = null!;
            IGrouping<string, GroupElement> group = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group, x => x
                    .Match<FactType2>(
                        f => f.TestProperty.StartsWith("Valid"),
                        f => f.JoinProperty == fact.TestProperty)
                    .GroupBy(f => GetKey(fact, f), f => new GroupElement(fact, f)));
            Then()
                .Do(ctx => ctx.NoOp());
        }

        private static string GetKey(FactType1 fact1, FactType2 fact2)
        {
            return $"{fact1.GroupKey}|{fact2.GroupKey}";
        }
    }
}
