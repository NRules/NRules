﻿using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactNoBindingRuleTest : BaseRulesTestFixture
{
    private TestRule _testRule;

    [Fact]
    public void Fire_OneMatchingFact_FiresOnce()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_OneMatchingFact_FactInContext()
    {
        //Arrange
        var fact = new FactType { TestProperty = "Valid Value 1" };
        Session.Insert(fact);

        IFactMatch[] matches = null;
        _testRule.Action = ctx =>
        {
            matches = ctx.Match.Facts.ToArray();
        };

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.Single(matches);
        Assert.Same(fact, matches[0].Value);
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        _testRule = new TestRule();
        setup.Rule(_testRule);
    }

    public class FactType
    {
        public string TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public Action<IContext> Action = ctx => { };

        public override void Define()
        {
            When()
                .Match<FactType>(f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => Action(ctx));
        }
    }
}