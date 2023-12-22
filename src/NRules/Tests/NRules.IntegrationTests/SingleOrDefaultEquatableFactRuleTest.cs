﻿using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class SingleOrDefaultEquatableFactRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactsAndOneInvalid_FiresOnceWithValidFact()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Invalid Value 1", ValueProperty = "Original 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2", ValueProperty = "Original 2" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactType>()
            .Callback(firedFact =>
            {
                Assert.Equal(2, firedFact.Id);
                Assert.Equal("Original 2", firedFact.ValueProperty);
            })));
    }

    [Fact]
    public void Fire_NoValidFacts_FiresOnceWithDefault()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Invalid Value 1", ValueProperty = "Original 1" };

        var facts = new[] { fact1 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactType>()
            .Callback(firedFact =>
            {
                Assert.Equal(0, firedFact.Id);
                Assert.Null(firedFact.ValueProperty);
            })));
    }

    [Fact]
    public void Fire_NoValidFactsUpdatedToValid_FiresOnceWithValidFact()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Invalid Value 1", ValueProperty = "Original 1" };

        var facts = new[] { fact1 };
        Session.InsertAll(facts);

        var fact11 = new FactType(1) { TestProperty = "Valid Value 1", ValueProperty = "Original 1" };
        Session.Update(fact11);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactType>()
            .Callback(firedFact =>
            {
                Assert.Equal(1, firedFact.Id);
                Assert.Equal("Original 1", firedFact.ValueProperty);
            })));
    }

    [Fact]
    public void Fire_ValidFactInsertedThenUpdated_FiresOnceWithUpdatedValue()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1", ValueProperty = "Original 1" };

        var facts = new[] { fact1 };
        Session.InsertAll(facts);

        var fact11 = new FactType(1) { TestProperty = "Valid Value 1", ValueProperty = "Updated 1" };
        Session.Update(fact11);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactType>()
            .Callback(firedFact =>
            {
                Assert.Equal(1, firedFact.Id);
                Assert.Equal("Updated 1", firedFact.ValueProperty);
            })));
    }

    [Fact]
    public void Fire_TwoValidFactsInsertedThenUpdated_FiresOnceThenFiresOnceAgain()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act - 1
        Session.Fire();

        //Assert - 1
        Verify(x => x.Rule().Fired());

        //Act - 2
        Session.Update(fact1);
        Session.Fire();

        //Assert - 2
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
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
        public string TestProperty { get; set; }
        public string ValueProperty { get; set; }

        public bool Equals(FactType other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
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
            FactType fact = null;

            When()
                .Query(() => fact, q => q
                    .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect()
                    .Select(x => x.OrderBy(f => f.Id).FirstOrDefault() ?? new FactType(0)));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}