using System;
using System.Diagnostics.CodeAnalysis;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneRecordFactRuleTest : BaseRulesTestFixture
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
        Verify(x => x.Rule().Fired());
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
        Verify(x => x.Rule().Fired(Times.Twice));
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
        Verify(x => x.Rule().Fired(Times.Never));
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
        Verify(x => x.Rule().Fired());
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
        Verify(x => x.Rule().Fired());
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
        Verify(x => x.Rule().Fired(Times.Never));
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
        Verify(x => x.Rule().Fired(Times.Never));
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

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public record FactType : IIdentityProvider
    {
        public FactType(int id)
        {
            Id = id;
        }

        public int Id { get; }
        [NotNull]
        public string? TestProperty { get; set; }

        public object GetIdentity() => Id;
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType fact = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}