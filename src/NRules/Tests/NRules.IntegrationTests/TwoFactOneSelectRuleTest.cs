using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactOneSelectRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactOfEachKind_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal($"{fact1.TestProperty}|{fact2.TestProperty}", GetFiredFact<FactProjection>()!.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindFirstFactUpdated_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        fact1.TestProperty = "Valid Value 11";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal($"{fact1.TestProperty}|{fact2.TestProperty}", GetFiredFact<FactProjection>()!.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindSecondFactUpdated_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.TestProperty = "Valid Value 22";
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal($"{fact1.TestProperty}|{fact2.TestProperty}", GetFiredFact<FactProjection>()!.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindFirstFactUpdatedInvalidJoin_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        fact1.JoinProperty = "Value 2";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindSecondFactUpdatedInvalidJoin_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.JoinProperty = "Value 2";
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindFirstFactRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindSecondFactRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Retract(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_TwoPairsOfMatchingFacts_FiresTwice()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 11", JoinProperty = "Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 12", JoinProperty = "Value 2" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 21", JoinProperty = "Value 1" };
        var fact22 = new FactType2 { TestProperty = "Valid Value 22", JoinProperty = "Value 2" };
        Session.InsertAll(new[] { fact11, fact12 });
        Session.InsertAll(new[] { fact21, fact22 });

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(2);
        Assert.Equal($"{fact11.TestProperty}|{fact21.TestProperty}", GetFiredFact<FactProjection>(0)!.Value);
        Assert.Equal($"{fact12.TestProperty}|{fact22.TestProperty}", GetFiredFact<FactProjection>(1)!.Value);
    }

    [Fact]
    public void Fire_TwoMatchingFactsOfOneKindOneFactOfSecondKindDuplicateProjectionsFirstSetUpdatedToInvalid_DoesNotFire()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.InsertAll(new object[] { fact11, fact12 });
        Session.Insert(fact2);

        fact11.TestProperty = "Invalid Value 1";
        Session.Update(fact11);
        fact12.TestProperty = "Invalid Value 1";
        Session.Update(fact12);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_TwoMatchingFactsOfOneKindOneFactOfSecondKindDuplicateProjectionsFirstSetRetracted_DoesNotFire()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Value 1" };
        Session.InsertAll(new object[] { fact11, fact12 });
        Session.Insert(fact2);

        Session.Retract(fact11);
        Session.Retract(fact12);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_MatchingFactOfFirstKind_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Value 1" };
        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_MatchingFactOfSecondKind_DoesNotFire()
    {
        //Arrange
        var fact2 = new FactType2 { TestProperty = "Valid Value 2" };
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string? TestProperty { get; set; }
        public string? JoinProperty { get; set; }
    }

    public class FactType2
    {
        public string? TestProperty { get; set; }
        public string? JoinProperty { get; set; }
    }

    public class FactProjection : IEquatable<FactProjection>
    {
        public FactProjection(FactType1 fact1, FactType2 fact2)
        {
            Value = $"{fact1.TestProperty}|{fact2.TestProperty}";
        }

        public string Value { get; }

        public bool Equals(FactProjection? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FactProjection)obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null!;
            FactProjection projection = null!;

            When()
                .Match(() => fact1, f => f.TestProperty!.StartsWith("Valid"))
                .Query(() => projection, q => q
                    .Match<FactType2>(f => f.JoinProperty == fact1.JoinProperty)
                    .Select(f => new FactProjection(fact1, f))
                    .Where(p => IsValid(p))
                    );
            Then()
                .Do(ctx => ctx.NoOp());
        }

        private static bool IsValid(FactProjection p)
        {
            return p.Value.StartsWith("Valid");
        }
    }
}