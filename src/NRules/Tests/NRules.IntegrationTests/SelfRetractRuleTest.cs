﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class SelfRetractRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_OneMatchingFact_FiresOnceAndRetractsFact()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var facts = new[] { fact1 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.Equal(0, Session.Query<FactType>().Count());
    }

    [Fact]
    public void Fire_NoMatchingFact_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Invalid Value 1" };
        var facts = new[] { fact1 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_TwoMatchingFacts_FiresTwiceAndRetractsFacts()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };
        var fact2 = new FactType { TestProperty = "Valid Value 2" };
        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Twice));
        Assert.Equal(0, Session.Query<FactType>().Count());
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType fact = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.TryRetract(fact));
        }
    }
}