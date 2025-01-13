﻿using System.Diagnostics.CodeAnalysis;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_MatchingFactsInsertAndFireThenUpdateAndFire_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();
        Session.Update(fact1);
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    [Fact]
    public void Fire_MatchingFactsInsertThenUpdateThenFire_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingFacts_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingFactsReverseOrder_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact2);
        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_TwoMatchingPairsOfFacts_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        var fact3 = new FactType1 { TestProperty = "Valid Value 3" };
        var fact4 = new FactType2 { TestProperty = "Valid Value 4", JoinProperty = fact3.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Insert(fact3);
        Session.Insert(fact4);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    [Fact]
    public void Fire_TwoMatchingFactsOfOneKindOneMatchingFactOfAnotherKind_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };
        var fact3 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);
        Session.Insert(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    [Fact]
    public void Fire_FirstMatchingFactSecondInvalidFact_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FirstInvalidFactSecondMatchingFact_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_TwoMatchingFactsUnsatisfiedJoin_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = null };

        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_TwoMatchingFactsFirstRetracted_DoesNotFire()
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
    public void Fire_TwoMatchingFactsSecondRetracted_DoesNotFire()
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
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_TwoMatchingFactsFirstUpdatedToInvalid_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact1.TestProperty = "Invalid Value 1";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_OneInvalidFactAndSecondMatchingFactFirstUpdatedToValid_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact1.TestProperty = "Valid Value 1";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_TwoMatchingFactsSecondUpdatedToInvalid_DoesNotFire()
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
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_OneMatchingFactSecondInvalidThenUpdatedToValid_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.TestProperty = "Valid Value 2";
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_OneMatchingFactSecondInvalidThenUpdatedToValidJoin_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = null };

        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.JoinProperty = fact1.TestProperty;
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
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
        [NotNull]
        public FactType1? Fact1 { get; set; }
        [NotNull]
        public FactType2? Fact2 = null!;

        public override void Define()
        {
            When()
                .Match(() => Fact1, f => f.TestProperty.StartsWith("Valid"))
                .Match(() => Fact2, f => f.TestProperty.StartsWith("Valid"))
                .Having(() => Fact2.JoinProperty == Fact1.TestProperty);

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}