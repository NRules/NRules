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
    public void Rewrite_LocalVariableAliasMatchingPropertyName_RewritesCorrectly()
    {
        // Arrange
        // ReSharper disable once InconsistentNaming - intentional to facilitate the test
        TestFact1 Fact1 = null!;
        var rewriter = CreateRewriter(() => Fact1);
        
        Expression<Func<TestFact2, bool>> condition = f => f.Fact1 == Fact1;
        Expression<Func<TestFact2, TestFact1, bool>> expected = (f, fact1) => f.Fact1 == fact1;
        
        // Act
        var actual = rewriter.Rewrite(condition);

        // Assert
        var comparer = new ExpressionComparer(RuleCompilerOptions.Default);
        Assert.True(comparer.AreEqual(expected, actual));
    }
    
    [Fact]
    public void Rewrite_PropertyAliasMatchingPropertyName_RewritesCorrectly()
    {
        // Arrange
        var wrapper = new Wrapper1();
        var rewriter = CreateRewriter(() => wrapper.Fact1);
        
        Expression<Func<TestFact2, bool>> condition = f => f.Fact1 == wrapper.Fact1;
        Expression<Func<TestFact2, TestFact1, bool>> expected = (f, fact1) => f.Fact1 == fact1;
        
        // Act
        var actual = rewriter.Rewrite(condition);

        // Assert
        var comparer = new ExpressionComparer(RuleCompilerOptions.Default);
        Assert.True(comparer.AreEqual(expected, actual));
    }
    
    [Fact]
    public void Rewrite_FieldAliasMatchingPropertyName_RewritesCorrectly()
    {
        // Arrange
        var wrapper = new Wrapper3();
        var rewriter = CreateRewriter(() => wrapper.Fact1);
        
        Expression<Func<TestFact2, bool>> condition = f => f.Fact1 == wrapper.Fact1;
        Expression<Func<TestFact2, TestFact1, bool>> expected = (f, fact1) => f.Fact1 == fact1;
        
        // Act
        var actual = rewriter.Rewrite(condition);

        // Assert
        var comparer = new ExpressionComparer(RuleCompilerOptions.Default);
        Assert.True(comparer.AreEqual(expected, actual));
    }
    
    [Fact]
    public void Rewrite_PropertyAliasMatchingPropertyNameWrongType_RewritesCorrectly()
    {
        // Arrange
        var wrapper1 = new Wrapper1();
        var wrapper2 = new Wrapper2();
        var rewriter = CreateRewriter(() => wrapper1.Fact1);
        
        Expression<Func<TestFact2, bool>> condition = f => Equals(f.Fact1, wrapper2.Fact1);
        Expression<Func<TestFact2, bool>> expected = f => Equals(f.Fact1, wrapper2.Fact1);
        
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

    public class TestFact1
    {
    }
    
    public class TestFact2
    {
        public TestFact1 Fact1 { get; set; } = null!;
    }
    
    public class Wrapper1
    {
        public TestFact1 Fact1 { get; set; } = null!;
    }
    
    public class Wrapper2
    {
        public TestFact2 Fact1 { get; set; } = null!;
    }
    
    public class Wrapper3
    {
        public TestFact1 Fact1 = null!;
    }
}