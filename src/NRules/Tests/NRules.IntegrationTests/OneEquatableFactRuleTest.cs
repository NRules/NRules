using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class OneEquatableFactRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFact_FiresOnce()
    {
        //Arrange
        var fact = new FactType(1) { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
    }

    [Fact]
    public void Fire_TwoMatchingFacts_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(2);
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(1) { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Retract(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneFactUpdatedFromInvalidToMatching_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Invalid Value 1" };
        var fact2 = new FactType(1) { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndRetractedAndAssertedAgain_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact3 = new FactType(1) { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Retract(fact2);
        Session.Insert(fact3);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndUpdatedToInvalid_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(1) { TestProperty = "Invalid Value 1" };
        Session.Insert(fact1);
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndModifiedAndRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(1) { TestProperty = "Invalid Value 1" };
        Session.Insert(fact1);
        Session.Retract(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_DuplicateInsert_Throws()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(1) { TestProperty = "Valid Value 2" };

        //Act - Assert
        Session.Insert(fact1);
        Assert.Throws<ArgumentException>(() => Session.Insert(fact2));
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType : IEquatable<FactType>
    {
        public FactType(int id)
        {
            Id = id;
        }

        public int Id { get; }
        public string? TestProperty { get; set; }

        public bool Equals(FactType? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FactType)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType? fact = null;

            When()
                .Match(() => fact, f => f!.TestProperty!.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}