﻿using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class OneEquatableFactOneCollectionRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoFactsInCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };
        var fact3 = new FactType(3) { TestProperty = "Invalid Value 3" };

        var facts = new[] { fact1, fact2, fact3 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
    }

    [Fact]
    public void Fire_OneMatchingFactInsertedThenUpdated_FiresOnceWithOneFactInCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact11 = new FactType(1) { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);
        Session.Update(fact11);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Single(GetFiredFact<IEnumerable<FactType>>());
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoFactsInCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };
        var fact21 = new FactType(2) { TestProperty = "Valid Value 2" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);
        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };
        var fact21 = new FactType(2) { TestProperty = "Valid Value 2" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);
        Session.Retract(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Single(GetFiredFact<IEnumerable<FactType>>());
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact11 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };
        var fact21 = new FactType(2) { TestProperty = "Valid Value 2" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);
        Session.Retract(fact11);
        Session.Retract(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Empty(GetFiredFact<IEnumerable<FactType>>());
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Valid Value 2" };
        var fact21 = new FactType(2) { TestProperty = "Invalid Value" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Single(GetFiredFact<IEnumerable<FactType>>());
    }

    [Fact]
    public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoFactInCollection()
    {
        //Arrange
        var fact1 = new FactType(1) { TestProperty = "Valid Value 1" };
        var fact2 = new FactType(2) { TestProperty = "Invalid Value" };
        var fact21 = new FactType(2) { TestProperty = "Valid Value 2" };

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
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
        public string TestProperty { get; set; }

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
            IEnumerable<FactType> collection = null;

            When()
                .Query(() => collection, q => q
                    .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect());
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}