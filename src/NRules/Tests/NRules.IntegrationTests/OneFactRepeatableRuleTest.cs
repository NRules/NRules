﻿using System.Diagnostics.CodeAnalysis;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactRepeatableRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFactEligibleForOneIncrement_FiresOnce()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1", TestCount = 2 };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_OneMatchingFactEligibleForTwoIncrements_FiresTwice()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1", TestCount = 1 };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        [NotNull]
        public string? TestProperty { get; set; }
        public int TestCount { get; set; }

        public void IncrementCount()
        {
            TestCount++;
        }
    }

    [Repeatability(RuleRepeatability.Repeatable)]
    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType fact = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"), f => f.TestCount <= 2);
            Then()
                .Do(ctx => fact.IncrementCount())
                .Do(ctx => ctx.Update(fact));
        }
    }
}