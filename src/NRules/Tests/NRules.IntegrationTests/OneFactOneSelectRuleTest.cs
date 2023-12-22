﻿using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneSelectRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFact_FiresOnce()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactProjection>(f => f.Value == fact.TestProperty)));
    }

    [Fact]
    public void Fire_OneMatchingFactInsertedThenUpdated_FiresOnce()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);
        Session.Update(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactProjection>(f => f.Value == fact.TestProperty)));
    }

    [Fact]
    public void Fire_TwoMatchingFacts_FiresTwice()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var fact2 = new FactType { TestProperty = "Valid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact<FactProjection>(f => f.Value == fact1.TestProperty));
            s.Rule().Fired(Matched.Fact<FactProjection>(f => f.Value == fact2.TestProperty));
        });
    }

    [Fact]
    public void Fire_ConditionDoesNotMatch_DoesNotFire()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Invalid Value 1" };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndRetracted_DoesNotFire()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);
        Session.Retract(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_OneFactUpdatedFromInvalidToMatching_FiresOnce()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Invalid Value 1" };
        Session.Insert(fact);

        fact.TestProperty = "Valid Value 1";
        Session.Update(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactProjection>(f => f.Value == fact.TestProperty)));
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndRetractedAndAssertedAgain_FiresOnce()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);
        Session.Retract(fact);
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactProjection>(f => f.Value == fact.TestProperty)));
    }

    [Fact]
    public void Fire_OneMatchingFactAssertedAndUpdatedToInvalid_DoesNotFire()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        fact.TestProperty = "Invalid Value 1";
        Session.Update(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        public string TestProperty { get; set; }
    }

    public class FactProjection : IEquatable<FactProjection>
    {
        public FactProjection(FactType fact)
        {
            Value = fact.TestProperty;
        }

        public string Value { get; }

        public bool Equals(FactProjection other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
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
            FactProjection projection = null;

            When()
                .Query(() => projection, x => x
                    .Match<FactType>()
                    .Select(f => new FactProjection(f))
                    .Where(p => p.Value.StartsWith("Valid")));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}