using System;
using System.Linq.Expressions;
using NRules.Fluent.Expressions;
using NRules.RuleModel;
using NRules.Utilities;
using Xunit;

namespace NRules.Tests.Fluent;

public class ExpressionRewriterTest
{
    [Fact]
    public void Rewrite_AliasMatchingPropertyName_RewritesCorrectly()
    {
        // Arrange
        // ReSharper disable once InconsistentNaming - intentional to facilitate the test
        string Fact1 = null!;
        var rewriter = CreateRewriter(() => Fact1);
        
        Expression<Func<TestFact, bool>> condition = f => f.Fact1 == Fact1;
        Expression<Func<TestFact, string, bool>> expected = (f, fact1) => f.Fact1 == fact1;
        
        // Act
        var actual = rewriter.Rewrite(condition);

        // Assert
        var comparer = new ExpressionComparer(RuleCompilerOptions.Default);
        Assert.True(comparer.AreEqual(expected, actual));
    }

    private static ExpressionRewriter CreateRewriter<TFact>(Expression<Func<TFact>> alias)
    {
        var symbol = alias.ToParameterExpression();
        var symbolTable = new SymbolTable();
        symbolTable.Add(new Declaration(symbol.Type, symbol.Name!));
        
        return new ExpressionRewriter(symbolTable);
    }

    public class TestFact
    {
        public string Fact1 { get; set; } = null!;
    }
}