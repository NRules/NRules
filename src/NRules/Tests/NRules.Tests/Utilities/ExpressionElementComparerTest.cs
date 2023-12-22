using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Utilities;
using Xunit;

namespace NRules.Tests.Utilities;

public class ExpressionElementComparerTest
{
    [Fact]
    public void AreEqual_SameNameSameExpression_True()
    {
        Expression<Func<string, string>> lambda1 = s => s;
        Expression<Func<string, string>> lambda2 = s => s;
        AssertEqual(Element.Expression("Name", lambda1), Element.Expression("Name", lambda2));
    }
    
    [Fact]
    public void AreEqual_DifferentNamesSameExpression_False()
    {
        Expression<Func<string, string>> lambda1 = s => s;
        Expression<Func<string, string>> lambda2 = s => s;
        AssertNotEqual(Element.Expression("Name1", lambda1), Element.Expression("Name2", lambda2));
    }
    
    [Fact]
    public void AreEqual_SameNameDifferentExpressions_False()
    {
        Expression<Func<string, string>> lambda1 = s => s.ToUpper();
        Expression<Func<string, string>> lambda2 = s => s.ToLower();
        AssertNotEqual(Element.Expression("Name", lambda1), Element.Expression("Name", lambda2));
    }

    [Fact]
    public void AreEqual_ListsOfEqualNamedExpressions_True()
    {
        Expression<Func<string, string>> lambda1 = s => s;
        Expression<Func<string, string>> lambda2 = s => s;
        AssertEqual(
            new List<Declaration>{Element.Declaration(typeof(string), "s")},
            new []{Element.Expression("Name", lambda1)},
            new List<Declaration>{Element.Declaration(typeof(string), "s")},
            new []{Element.Expression("Name", lambda2)});
    }

    [Fact]
    public void AreEqual_ListsOfNonEqualNamedExpressions_False()
    {
        Expression<Func<string, string>> lambda1 = s => s.ToUpper();
        Expression<Func<string, string>> lambda2 = s => s.ToLower();
        AssertNotEqual(
            new List<Declaration>{Element.Declaration(typeof(string), "s")},
            new []{Element.Expression("Name", lambda1)},
            new List<Declaration>{Element.Declaration(typeof(string), "s")},
            new []{Element.Expression("Name", lambda2)});
    }

    [Fact]
    public void AreEqual_ListsOfNamedExpressionsFirstEmpty_False()
    {
        Expression<Func<string, string>> lambda2 = s => s;
        AssertNotEqual(
            new List<Declaration>(),
            Array.Empty<NamedExpressionElement>(),
            new List<Declaration>{Element.Declaration(typeof(string), "s")},
            new []{Element.Expression("Name", lambda2)});
    }

    [Fact]
    public void AreEqual_ListsOfNamedExpressionsSecondEmpty_False()
    {
        Expression<Func<string, string>> lambda1 = s => s;
        AssertNotEqual(
            new List<Declaration>{Element.Declaration(typeof(string), "s")},
            new []{Element.Expression("Name", lambda1)},
            new List<Declaration>(),
            Array.Empty<NamedExpressionElement>());
    }

    private void AssertEqual(NamedExpressionElement first, NamedExpressionElement second)
    {
        //Act
        var target = CreateTarget();
        bool result = target.AreEqual(first, second);

        //Assert
        Assert.True(result);
    }

    private void AssertNotEqual(NamedExpressionElement first, NamedExpressionElement second)
    {
        //Act
        var target = CreateTarget();
        bool result = target.AreEqual(first, second);

        //Assert
        Assert.False(result);
    }

    private void AssertEqual(List<Declaration> declarationsFirst, IEnumerable<NamedExpressionElement> first, List<Declaration> declarationsSecond, IEnumerable<NamedExpressionElement> second)
    {
        //Act
        var target = CreateTarget();
        bool result = target.AreEqual(declarationsFirst, first, declarationsSecond, second);

        //Assert
        Assert.True(result);
    }

    private void AssertNotEqual(List<Declaration> declarationsFirst, IEnumerable<NamedExpressionElement> first, List<Declaration> declarationsSecond, IEnumerable<NamedExpressionElement> second)
    {
        //Act
        var target = CreateTarget();
        bool result = target.AreEqual(declarationsFirst, first, declarationsSecond, second);

        //Assert
        Assert.False(result);
    }

    private ExpressionElementComparer CreateTarget()
    {
        var options = new RuleCompilerOptions();
        return new ExpressionElementComparer(options);
    }
}
