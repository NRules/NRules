using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOneCollectionRuleTest : BaseRulesTestFixture
{
    private TestRule _testRule = null!;

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnother_FiresOnceWithTwoFactsInCollection()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = null };
        var fact4 = new FactType2 { TestProperty = "Invalid Value 4", JoinProperty = fact1.TestProperty };
        var fact5 = new FactType2 { TestProperty = "Valid Value 5", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3, fact4, fact5 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 2)));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndNoneOfAnother_FiresOnceWithEmptyCollection()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => !f.Any())));
    }

    [Fact]
    public void Fire_OneMatchingSetOfFacts_FactsInContext()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        IFactMatch[]? matches = null;
        _testRule.Action = ctx =>
        {
            matches = ctx.Match.Facts.ToArray();
        };

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.NotNull(matches);
        Assert.Equal(2, matches.Length);
        Assert.Equal("fact", matches[0].Declaration.Name);
        Assert.Same(fact1, matches[0].Value);
        Assert.Equal("collection", matches[1].Declaration.Name);
        Assert.Equal(new[] { fact2 }, (IEnumerable<FactType2>?)matches[1].Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherThenFireThenAnotherMatchingFactThenFire_FiresOnceWithTwoFactsInCollectionThenFiresAgainWithThreeFacts()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 21", JoinProperty = fact1.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value 22", JoinProperty = fact1.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value 23", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 2)));

        //Act
        Recorder.Clear();
        Session.Insert(fact23);
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 3)));
    }

    [Fact]
    public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherThenAnotherMatchingFactThenFire_FiresOnceWithThreeFactsInCollection()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 21", JoinProperty = fact1.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value 22", JoinProperty = fact1.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value 23", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        var facts = new[] { fact21, fact22, fact23 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 3)));
    }

    [Fact]
    public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenOneRetracted_FiresOnceWithOneFactInCollection()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3 };
        Session.InsertAll(facts);

        Session.Retract(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 1)));
    }

    [Fact]
    public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenRetracted_FiresOnceWithEmptyCollection()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        var facts = new[] { fact2, fact3 };
        Session.InsertAll(facts);

        Session.Retract(fact2);
        Session.Retract(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => !f.Any())));
    }

    [Fact]
    public void Fire_FactOfOneKindIsInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Invalid Value 3", JoinProperty = fact1.TestProperty };
        var fact4 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact1.TestProperty };

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
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Invalid Value 3", JoinProperty = fact1.TestProperty };
        var fact4 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact1.TestProperty };

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
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Invalid Value 3", JoinProperty = fact1.TestProperty };
        var fact4 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact1.TestProperty };

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
    public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingOneOfTheFacts_FiresOnceWithTwoFactsAndOnceWithEmptyCollection()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact11.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        Session.Insert(fact21);
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 2));
            s.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => !f.Any()));
        });
    }

    [Fact]
    public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFacts_FiresTwiceWithCorrectCounts()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value 5", JoinProperty = fact12.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        var facts = new[] { fact21, fact22, fact23 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 2));
            s.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(f => f.Count() == 1));
        });
    }

    [Fact]
    public void Fire_TwoMatchedSetsThenOneFactOfFirstKindUpdated_FiresTwiceThenFiresOnce()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value 5", JoinProperty = fact12.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        var facts = new[] { fact21, fact22, fact23 };
        Session.InsertAll(facts);

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
    public void Fire_TwoMatchedSetsThenOneFactOfSecondKindUpdated_FiresTwiceThenFiresOnce()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty };
        var fact22 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact11.TestProperty };
        var fact23 = new FactType2 { TestProperty = "Valid Value 5", JoinProperty = fact12.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        var facts = new[] { fact21, fact22, fact23 };
        Session.InsertAll(facts);

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

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        _testRule = new TestRule();
        setup.Rule(_testRule);
    }

    public class FactType1
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class FactType2
    {
        [NotNull]
        public string? TestProperty { get; set; }
        public string? JoinProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public Action<IContext> Action = ctx => { };

        public override void Define()
        {
            FactType1 fact = null!;
            IEnumerable<FactType2> collection = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => collection, x => x
                    .Match<FactType2>(
                        f => f.TestProperty.StartsWith("Valid"),
                        f => f.JoinProperty == fact.TestProperty)
                    .Collect());
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}