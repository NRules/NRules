﻿using System.Diagnostics.CodeAnalysis;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOneNotRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_MatchingNotPatternFacts_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_InsertedThenRetractedFactMatchingNotPatternFacts_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_MatchingNotPatternFactAssertedThenRetracted_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Retract(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingNotPatternFactAssertedThenUpdatedToInvalid_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.TestProperty = "Invalid Value 2";
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingFactInsertedThenRetractedAndNoFactsMatchingNotPattern_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);
        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_MatchingFactAndNoFactsMatchingNotPattern_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingFactInsertedThenUpdatedAndNoFactsMatchingNotPattern_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_TwoMatchingFactsAndNoFactsMatchingNotPattern_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType1 { TestProperty = "Valid Value 2" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    [Fact]
    public void Fire_TwoMatchingFactsAndOneFactMatchingNotPattern_FiresOnce()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty };

        Session.Insert(fact11);
        Session.Insert(fact12);
        Session.Insert(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_TwoMatchingSetsFactOfSecondKindInsertedThenRetracted_FiresTwiceThenFiresTwice()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value" };
        var fact12 = new FactType1 { TestProperty = "Valid Value" };
        var fact21 = new FactType2 { TestProperty = "Valid Value", JoinProperty = "Valid Value" };

        var facts = new[] { fact11, fact12 };
        Session.InsertAll(facts);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify(x => x.Rule().Fired(Times.Twice));

        //Act - 2
        Session.Insert(fact21);
        Session.Retract(fact21);
        Session.Fire();

        //Assert - 2
        Verify(x => x.Rule().Fired(Times.Exactly(4)));
    }

    [Fact]
    public void Fire_TwoMatchingSetsFactOfFirstKindUpdated_FiresTwiceThenFiresOnce()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value" };
        var fact12 = new FactType1 { TestProperty = "Valid Value" };

        var facts = new[] { fact11, fact12 };
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
        public string? TestProperty { get; set; }
        [NotNull]
        public string? JoinProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Not<FactType2>(f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact.TestProperty);
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}