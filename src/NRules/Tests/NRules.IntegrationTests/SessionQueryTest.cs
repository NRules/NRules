﻿using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class SessionQueryTest : BaseRulesTestFixture
{
    [Fact]
    public void Query_NoFacts_Empty()
    {
        //Arrange
        //Act
        var query = Session.Query<object>();
        var facts = query.ToList();

        //Assert
        Assert.Empty(facts);
    }

    [Fact]
    public void Query_OneFact_RetrievesFactFromQuery()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);

        //Act
        var query = Session.Query<object>();
        var facts = query.ToList();

        //Assert
        Assert.Single(facts);
        Assert.Same(fact1, facts[0]);
    }

    [Fact]
    public void Query_RuleInsertsSecondFact_TwoFactsInMemory()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Fire();

        //Act
        var query = Session.Query<object>();
        var facts = query.ToList();

        //Assert
        Assert.Equal(2, facts.Count);
    }

    [Fact]
    public void Query_QueryFactsByType_OnlyReturnsFactsOfThatType()
    {
        //Arrange
        var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact1);
        Session.Fire();

        //Act
        var query = Session.Query<FactType2>();
        var facts = query.ToList();

        //Assert
        Assert.Single(facts);
        Assert.Equal(fact1.TestProperty, facts[0].JoinProperty);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType1
    {
        public string TestProperty { get; set; }
        public string JoinProperty { get; set; }
    }

    public class FactType2
    {
        public string TestProperty { get; set; }
        public string JoinProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null;

            When()
                .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"));
            Then()
                .Do(ctx => ctx.Insert(new FactType2()
                {
                    TestProperty = fact1.JoinProperty,
                    JoinProperty = fact1.TestProperty
                }));
        }
    }
}