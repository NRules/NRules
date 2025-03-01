using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests;

public class RuleAliasResolutionTest
{
    private readonly RuleRepository _repository = new();

    [Fact]
    public void Rule_VariableAsAlias_DefinitionCreated()
    {
        //Arrange
        _repository.Load(x => x.NestedTypes().From(typeof(RuleWithVariableAlias)));

        //Act
        IRuleDefinition rule = _repository.GetRules().Single();

        //Assert
        Assert.Equal(2, rule.LeftHandSide.Exports.Count);
        Assert.Equal("fact1", rule.LeftHandSide.Exports.ElementAt(0).Name);
        Assert.Equal("fact2", rule.LeftHandSide.Exports.ElementAt(1).Name);
    }
    
    [Fact]
    public void Rule_PropertyAsAlias_DefinitionCreated()
    {
        //Arrange
        _repository.Load(x => x.NestedTypes().From(typeof(RuleWithPropertyAlias)));

        //Act
        IRuleDefinition rule = _repository.GetRules().Single();

        //Assert
        Assert.Equal(2, rule.LeftHandSide.Exports.Count);
        Assert.Equal("Fact1", rule.LeftHandSide.Exports.ElementAt(0).Name);
        Assert.Equal("Fact2", rule.LeftHandSide.Exports.ElementAt(1).Name);
    }
    
    [Fact]
    public void Rule_StaticPropertyAsAlias_DefinitionCreated()
    {
        //Arrange
        _repository.Load(x => x.NestedTypes().From(typeof(RuleWithStaticPropertyAlias)));

        //Act
        IRuleDefinition rule = _repository.GetRules().Single();

        //Assert
        Assert.Equal(2, rule.LeftHandSide.Exports.Count);
        Assert.Equal("Fact1", rule.LeftHandSide.Exports.ElementAt(0).Name);
        Assert.Equal("Fact2", rule.LeftHandSide.Exports.ElementAt(1).Name);
    }

    public class FactType1
    {
        [NotNull]
        public string? Value { get; set; }
    }
    
    public class FactType2
    {
        [NotNull]
        public FactType1? Fact { get; set; }
    }

    public class RuleWithVariableAlias : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null!;
            FactType2 fact2 = null!;

            When()
                .Match(() => fact1)
                .Match(() => fact2, f => f.Fact == fact1);
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    public class RuleWithPropertyAlias : Rule
    {
        public FactType1 Fact1 { get; set; } = null!;
        public FactType2 Fact2 { get; set; } = null!;
        
        public override void Define()
        {
            When()
                .Match(() => Fact1)
                .Match(() => Fact2, f => f.Fact == Fact1);
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
    
    public class RuleWithStaticPropertyAlias : Rule
    {
        public static FactType1 Fact1 { get; set; } = null!;
        public static FactType2 Fact2 { get; set; } = null!;
        
        public override void Define()
        {
            When()
                .Match(() => Fact1)
                .Match(() => Fact2, f => f.Fact == Fact1);
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}
