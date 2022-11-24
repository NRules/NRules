﻿using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactAggregateJoinRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_NoMatchingFacts_DoesNotFire()
    {
        //Arrange - Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(0);
    }

    [Fact]
    public void Fire_OneMatchingFact_FiresOnceWithOneFactInCollection()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Single(GetFiredFact<IEnumerable<FactType>>());
    }

    [Fact]
    public void Fire_OneMatchingFactTwoFactsToAggregate_FiresOnceWithTwoFactsInCollection()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var fact2 = new FactType { TestProperty = "Invalid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(1);
        Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
    }

    [Fact]
    public void Fire_TwoMatchingFactsTwoFactsToAggregate_FiresTwiceWithTwoFactsInEachCollection()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var fact2 = new FactType { TestProperty = "Valid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule().FiredTimes(2);
        Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>(0).Count());
        Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>(1).Count());
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        public string TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType fact = null;
            IEnumerable<FactType> collection = null;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => collection, x => x
                    .Match<FactType>()
                    .Collect()
                    .Where(c => c.Contains(fact)));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}
