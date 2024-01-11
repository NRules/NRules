using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class RuleNormalizationTest : RulesTestFixture
{
    private readonly RuleDefinitionFactory _factory;
    private readonly RuleNormalization _normalization;

    public RuleNormalizationTest()
    {
        _factory = new RuleDefinitionFactory();
        _normalization = new RuleNormalization();
    }
    
    [Fact]
    public void Normalize_NotNeeded_SameRule()
    {
        // Arrange
        var rule = _factory.Create(new NoNormalizationNecessaryTestRule());
        
        // Act
        var normalizedRuleDefinition = _normalization.Normalize(rule);
        
        // Assert
        Assert.Same(rule, normalizedRuleDefinition);
    }
    
    [Fact]
    public void Normalize_NestedOr_Rewrites()
    {
        // Arrange
        var rule = _factory.Create(new NestedOrTestRule());
        
        // Act
        var normalizedRuleDefinition = _normalization.Normalize(rule);
        
        // Assert
        Assert.NotSame(rule, normalizedRuleDefinition);
    }
    
    [Fact]
    public void Normalize_OneChildGroup_Rewrites()
    {
        // Arrange
        var rule = _factory.Create(new OneChildGroupTestRule());
        
        // Act
        var normalizedRuleDefinition = _normalization.Normalize(rule);
        
        // Assert
        Assert.NotSame(rule, normalizedRuleDefinition);
    }
    
    [Fact]
    public void Normalize_SplittableOrGroup_Rewrites()
    {
        // Arrange
        var rule = _factory.Create(new SplittableOrGroupTestRule());
        
        // Act
        var normalizedRuleDefinition = _normalization.Normalize(rule);
        
        // Assert
        Assert.NotSame(rule, normalizedRuleDefinition);
    }
    
    [Fact]
    public void Normalize_ForAll_Rewrites()
    {
        // Arrange
        var rule = _factory.Create(new ForAllTestRule());
        
        // Act
        var normalizedRuleDefinition = _normalization.Normalize(rule);
        
        // Assert
        Assert.NotSame(rule, normalizedRuleDefinition);
    }
    
    public class FactType1
    {
        public string Key { get; set; }
        public string Join { get; set; }
    }

    public class FactType2
    {
        public string Join { get; set; }
    }

    public class FactType3
    {
        public string Join { get; set; }
    }

    public class NoNormalizationNecessaryTestRule : Rule
    {
        public override void Define()
        {
            IGrouping<string, FactType1> group = default;
            IEnumerable<FactType2> facts2 = default;
            FactType3 fact3 = default;
            string calc4 = default;

            When()
                .Query(() => group, x => x
                    .Match<FactType1>()
                    .GroupBy(f => f.Key)
                    .Select(g => g)
                    .Where(g => g.Count() > 1))
                .Query(() => facts2, q => q
                    .Match<FactType2>(f => f.Join == group.First().Join)
                    .Collect())
                .Match(() => fact3, f => f.Join == group.First().Join)
                .Let(() => calc4, () => group.Key);

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    public class NestedOrTestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = default;
            FactType2 fact2 = default;
            FactType3 fact3 = default;

            When()
                .Or(x => x
                    .Or(y => y
                        .Match(() => fact1, f => f.Key == "key1")
                        .Match(() => fact2))
                    .Match(() => fact3));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    public class OneChildGroupTestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = default;
            FactType2 fact2 = default;

            When()
                .And(x => x
                    .And(y => y
                        .Match(() => fact1, f => f.Key == "key1")
                        .Match(() => fact2)));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    public class SplittableOrGroupTestRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = default;
            FactType2 fact2 = default;
            FactType3 fact3 = default;

            When()
                .Or(x => x
                    .And(y => y
                        .Match(() => fact1, f => f.Key == "key1")
                        .Match(() => fact2))
                    .Match(() => fact3));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    public class ForAllTestRule : Rule
    {
        public override void Define()
        {
            When()
                .All<FactType1>(
                    f => true,
                    f => f.Key == "key1");

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}