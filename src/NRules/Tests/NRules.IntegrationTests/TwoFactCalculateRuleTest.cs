using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class TwoFactCalculateRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactOfEachKind_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal("Valid Value 1|Valid Value 2", GetFiredFact<CalculatedFact3>()!.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindNullFact_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", ShouldProduceNull3 = true };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Null(GetFiredFact<CalculatedFact3>());
        Assert.NotNull(GetFiredFact<CalculatedFact4>());
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindNullFactFilteredOut_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", ShouldProduceNull4 = true };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindSecondFactUpdated_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.TestProperty = "Valid Value 22";
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal("Valid Value 1|Valid Value 22", GetFiredFact<CalculatedFact3>()!.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindFireThenSecondFactUpdated_FiresTwice()
    {
        //Arrange - 1
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify.Rule().FiredTimes(1);
        Assert.Equal("Valid Value 1|Valid Value 2", GetFiredFact<CalculatedFact3>(0)!.Value);

        //Arrange - 2
        fact2.TestProperty = "Valid Value 22";
        Session.Update(fact2);

        //Act - 2
        Session.Fire();

        //Assert - 2
        Verify.Rule().FiredTimes(2);
        Assert.Equal("Valid Value 1|Valid Value 22", GetFiredFact<CalculatedFact3>(1)!.Value);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindSecondFactUpdatedToInvalidateBindingCondition_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        fact2.Counter = 1;
        Session.Update(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFactOfEachKindSecondFactRetracted_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
        var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Insert(fact2);

        Session.Retract(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_TwoMatchingSetsOfFacts_FiresTwiceCalculatesPerSet()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 11" };
        var fact21 = new FactType2 { TestProperty = "Valid Value 21", JoinProperty = "Valid Value 11" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 12" };
        var fact22 = new FactType2 { TestProperty = "Valid Value 22", JoinProperty = "Valid Value 12" };
        Session.InsertAll(new[] { fact11, fact12 });
        Session.InsertAll(new[] { fact21, fact22 });

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(2);
        Assert.Equal("Valid Value 11|Valid Value 21", GetFiredFact<CalculatedFact3>(0)!.Value);
        Assert.Equal("Valid Value 12|Valid Value 22", GetFiredFact<CalculatedFact3>(1)!.Value);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string? TestProperty { get; set; }
        public int Counter { get; set; }
        public bool ShouldProduceNull3 { get; set; }
        public bool ShouldProduceNull4 { get; set; }
    }

    public class FactType2
    {
        public string? TestProperty { get; set; }
        public int Counter { get; set; }
        public string? JoinProperty { get; set; }
    }

    public class CalculatedFact3
    {
        public CalculatedFact3(FactType1 fact1, FactType2 fact2)
        {
            Value = $"{fact1.TestProperty}|{fact2.TestProperty}";
        }

        public string Value { get; }
    }

    public class CalculatedFact4
    {
        public CalculatedFact4(FactType1 fact1, FactType2 fact2)
        {
            Value = fact1.Counter + fact2.Counter;
        }

        public long Value { get; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null!;
            FactType2 fact2 = null!;
            CalculatedFact3? fact3 = null;
            CalculatedFact4? fact4 = null;

            When()
                .Match(() => fact1, f => f.TestProperty!.StartsWith("Valid"))
                .Match(() => fact2, f => f.TestProperty!.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                .Let(() => fact3, () => CreateFact3(fact1, fact2))
                .Let(() => fact4, () => CreateFact4(fact1, fact2))
                .Having(() => fact4 != null && fact4.Value == 0);

            Then()
                .Do(ctx => ctx.NoOp());
        }

        private static CalculatedFact3? CreateFact3(FactType1 fact1, FactType2 fact2)
        {
            if (fact1.ShouldProduceNull3)
                return null;
            return new CalculatedFact3(fact1, fact2);
        }

        private static CalculatedFact4? CreateFact4(FactType1 fact1, FactType2 fact2)
        {
            if (fact1.ShouldProduceNull4)
                return null;
            return new CalculatedFact4(fact1, fact2);
        }
    }
}
