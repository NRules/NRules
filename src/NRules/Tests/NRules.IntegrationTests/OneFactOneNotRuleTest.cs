﻿using System.Diagnostics.CodeAnalysis;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneNotRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_MatchingNotPatternFact_DoesNotFire()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_MatchingNotPatternFactAssertedThenRetracted_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);
        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_MatchingNotPatternFactAssertedThenUpdatedToInvalid_FiresOnce()
    {
        //Arrange
        var fact1 = new FactType { TestProperty = "Valid Value 1" };

        Session.Insert(fact1);

        fact1.TestProperty = "Invalid Value 1";
        Session.Update(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_NoFactMatchingNotPattern_FiresOnce()
    {
        //Arrange
        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
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
            When()
                .Not<FactType>(f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}