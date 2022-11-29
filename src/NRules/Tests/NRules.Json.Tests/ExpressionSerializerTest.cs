using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Tests.TestAssets;
using NRules.Json.Tests.Utilities;
using Xunit;

namespace NRules.Json.Tests;

public class ExpressionSerializerTest
{
    private readonly JsonSerializerOptions _options;

    public ExpressionSerializerTest()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        RuleSerializer.Setup(_options);
    }

    [Fact]
    public void Roundtrip_Constant_Equals()
    {
        Expression<Func<string>> expression = () => "Value";
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_PropertyAccess_Equals()
    {
        Expression<Func<string, int>> expression = s => s.Length;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_InstanceMethod_Equals()
    {
        Expression<Func<string, string>> expression = s => s.ToUpper();
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_StaticMethod_Equals()
    {
        Expression<Func<string, string>> expression = s => string.Concat(s, s);
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_StaticMethodOnInterface_Equals()
    {
        Expression<Func<FactType1, bool>> expression = f => Calculations.CallOnInterface(f);
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_ExtensionMethod_Equals()
    {
        Expression<Func<string, string>> expression = s => s.Transform();
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BinaryExpressionEqualsOperator_Equals()
    {
        Expression<Func<string, string, bool>> expression = (s1, s2) => s1 == s2;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BinaryExpressionEqualsBuiltIn_Equals()
    {
        Expression<Func<int, int, bool>> expression = (i1, i2) => i1 == i2;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BinaryExpressionArithmeticOps_Equals()
    {
        Expression<Func<double, double, double, double, double>> expression = (d1, d2, d3, d4)
            => ((d1 + d2 - d3) * d4) / d2 % 3;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BinaryExpressionArrayIndex_Equals()
    {
        Expression<Func<int[], int>> expression = arr => arr[0];
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BinaryExpressionAndOr_Equals()
    {
        Expression<Func<bool, bool, bool, bool>> expression = (a, b, c) => (a || b) && c;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BinaryExpressionStringConcatenation_Equals()
    {
        Expression<Func<string, string, string>> expression = (s1, s2) => s1 + s2;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_UnaryExpressionNot_Equals()
    {
        Expression<Func<bool, bool>> expression = a => !a;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_UnaryExpressionConvert_Equals()
    {
        Expression<Func<object, string>> expression = o => (string)o;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_UnaryExpressionTypeAs_Equals()
    {
        Expression<Func<object, string?>> expression = o => o as string;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_TypeBinaryExpressionTypeIs_Equals()
    {
        Expression<Func<object, bool>> expression = o => o is string;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_NewExpression_Equals()
    {
        Expression<Func<string, FactType3>> expression = s => new FactType3(s);
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_MemberInit_Equals()
    {
        Expression<Func<bool, string, FactType1>> expression = (b, s) => new FactType1
        { BooleanProperty = b, StringProperty = s };
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_ListInit_Equals()
    {
        Expression<Func<string, string, List<string>>> expression = (s1, s2) => new List<string> { s1, s2 };
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_NewArrayExpression_Equals()
    {
        Expression<Func<string, string[]>> expression = s => new[] { s };
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_InvocationExpression_Equals()
    {
        Expression<Func<string, string>> expression = s => Calculations.Concat(s, "Value");
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_NestedLambda_Equals()
    {
        Expression<Func<string, string>> expression = s => Calculations.Transform(s, x => x);
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_ConditionalExpression_Equals()
    {
        Expression<Func<int, int>> expression = i => i > 0 ? 1 : 0;
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_DefaultExpression_Equals()
    {
        var expression = Expression.Default(typeof(string));
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_BlockExpression_Equals()
    {
        var parameter = Expression.Parameter(typeof(string), "s");
        var expression = Expression.Lambda<Func<string, string>>(
            Expression.Block(
                parameter
                ),
            parameter);
        TestRoundtrip(expression);
    }

    [Fact]
    public void Roundtrip_AssignExpression_Equals()
    {
        var parameter = Expression.Parameter(typeof(string), "s");
        var expression = Expression.Lambda<Action<string>>(
            Expression.Assign(
                parameter,
                Expression.Constant("Constant")),
            parameter);
        TestRoundtrip(expression);
    }

    [Fact]
    public void Deserialize_WrongPropertyPosition_Throws()
    {
        var jsonString = @"{
  'nodeType': 'Constant',
  'value': 'My Value'
}".Replace('\'', '\"');
        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Expression>(jsonString, _options));
        Assert.Contains("Expected property. Name=Type", ex.Message);
    }

    [Fact]
    public void Deserialize_WrongEnumValue_Throws()
    {
        var jsonString = @"{
  'nodeType': 'InvalidValue'
}".Replace('\'', '\"');
        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Expression>(jsonString, _options));
        Assert.Contains("Unable to convert Enum value. Value=InvalidValue, EnumType=System.Linq.Expressions.ExpressionType", ex.Message);
    }

    private void TestRoundtrip<TExpression>(TExpression expression) where TExpression : Expression
    {
        var jsonString = JsonSerializer.Serialize(expression, _options);
        //System.IO.File.WriteAllText(@"C:\temp\expression.json", jsonString);
        var deserialized = JsonSerializer.Deserialize<TExpression>(jsonString, _options);

        Assert.True(ExpressionComparer.AreEqual(expression, deserialized));
    }
}
