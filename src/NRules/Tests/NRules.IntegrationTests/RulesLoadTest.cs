﻿using System;
using System.Linq;
using System.Reflection;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests;

public class RulesLoadTest
{
    [Fact]
    public void Load_AssemblyWithoutRules_Empty()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act
        target.Load(x => x.From(typeof(string).Assembly));
        IRuleSet ruleSet = target.GetRuleSets().First();

        //Assert
        Assert.Empty(ruleSet.Rules);
    }

    [Fact]
    public void Load_InvalidTypes_Empty()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act
        target.Load(x => x.From(typeof(string)));
        IRuleSet ruleSet = target.GetRuleSets().First();

        //Assert
        Assert.Empty(ruleSet.Rules);
    }

    [Fact]
    public void Load_EmptyRule_Throws()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act - Assert
        var ex = Assert.Throws<RuleDefinitionException>(() => target.Load(x => x.NestedTypes().From(typeof(EmptyRule))));
        Assert.Equal(typeof(EmptyRule), ex.RuleType);
    }

    [Fact]
    public void Load_NoActionRule_Throws()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act - Assert
        var ex = Assert.Throws<RuleDefinitionException>(() => target.Load(x => x.NestedTypes().From(typeof(NoActionRule))));
        Assert.Equal(typeof(NoActionRule), ex.RuleType);
    }

    [Fact]
    public void Load_CannotActivateRule_Throws()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act - Assert
        var ex = Assert.Throws<RuleActivationException>(() => target.Load(x => x.NestedTypes().From(typeof(CannotActivateRule))));
        Assert.Equal(typeof(CannotActivateRule), ex.RuleType);
    }

    [Fact]
    public void Load_AssemblyWithRulesToNamedRuleSet_RuleSetNameMatches()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act
        target.Load(x => x
            .NestedTypes()
            .From(ThisAssembly)
            .Where(r => r.RuleType == typeof(ValidRule))
            .To("Test"));
        IRuleSet ruleSet = target.GetRuleSets().First();

        //Assert
        Assert.Equal("Test", ruleSet.Name);
    }

    [Fact]
    public void Load_AssemblyWithRulesToDefaultRuleSet_DefaultRuleSetName()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act
        target.Load(x => x
            .NestedTypes()
            .From(ThisAssembly)
            .Where(r => r.RuleType == typeof(ValidRule)));
        IRuleSet ruleSet = target.GetRuleSets().First();

        //Assert
        Assert.Equal("default", ruleSet.Name);
    }

    [Fact]
    public void Load_FilterRuleByName_MatchingRule()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act
        target.Load(x => x
            .NestedTypes()
            .From(ThisAssembly)
            .Where(r => r.Name.Contains("Valid")));
        IRuleSet ruleSet = target.GetRuleSets().First();

        //Assert
        Assert.Single(ruleSet.Rules);
        Assert.Equal(typeof(ValidRule).FullName, ruleSet.Rules.First().Name);
    }

    [Fact]
    public void Load_FilterRuleByTag_MatchingRule()
    {
        //Arrange
        RuleRepository target = CreateTarget();

        //Act
        target.Load(x => x
            .NestedTypes()
            .From(ThisAssembly)
            .Where(r => r.IsTagged("LoadTest")));
        IRuleSet ruleSet = target.GetRuleSets().First();

        //Assert
        Assert.Single(ruleSet.Rules);
        Assert.Equal("Rule with metadata", ruleSet.Rules.First().Name);
    }

    [Fact]
    public void Add_RuleInstanceInRuleSet_AddedToRepository()
    {
        //Arrange
        var rule = new ValidRule();

        var factory = new RuleDefinitionFactory();
        var ruleDefinition = factory.Create(rule);
        
        var ruleSet = new RuleSet("MyRuleSet");
        ruleSet.Add(ruleDefinition);

        RuleRepository target = CreateTarget();

        //Act
        target.Add(ruleSet);

        //Assert
        Assert.Single(ruleSet.Rules);
        Assert.Equal(typeof(ValidRule).FullName, ruleSet.Rules.First().Name);
    }

    private Assembly ThisAssembly => GetType().Assembly;

    public RuleRepository CreateTarget()
    {
        return new RuleRepository();
    }

    public class FactType
    {
    }

    public class CannotActivateRule : Rule
    {
        public CannotActivateRule()
        {
            throw new InvalidOperationException("Failed in ctor");
        }

        public override void Define()
        {
        }
    }

    public class EmptyRule : Rule
    {
        public override void Define()
        {
        }
    }

    public class NoActionRule : Rule
    {
        public override void Define()
        {
            When()
                .Match<FactType>();
        }
    }

    public class ValidRule : Rule
    {
        public override void Define()
        {
            When()
                .Match<FactType>();
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }

    [Name("Rule with metadata")]
    [Tag("LoadTest")]
    public class TaggedRule : Rule
    {
        public override void Define()
        {
            When()
                .Match<FactType>();
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}