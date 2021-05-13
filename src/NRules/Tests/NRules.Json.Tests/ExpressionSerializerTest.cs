using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Tests.TestAssets;
using NRules.Json.Tests.Utilities;
using Xunit;

namespace NRules.Json.Tests
{
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
            Expression<Func<string, IEnumerable<char>>> expression = s => (IEnumerable<char>)s;
            TestRoundtrip(expression);
        }

        [Fact]
        public void Roundtrip_UnaryExpressionTypeAs_Equals()
        {
            Expression<Func<string, IEnumerable<char>>> expression = s => s as IEnumerable<char>;
            TestRoundtrip(expression);
        }

        [Fact]
        public void Roundtrip_InvocationExpression_Equals()
        {
            Expression<Func<string, string>> expression = s => Concat(s, "Value");
            TestRoundtrip(expression);
        }

        public static TransformDelegate Concat = string.Concat;

        private void TestRoundtrip<TExpression>(TExpression expression) where TExpression: Expression
        {
            var jsonString = JsonSerializer.Serialize(expression, _options);
            //System.IO.File.WriteAllText(@"C:\temp\expression.json", jsonString);
            var deserialized = JsonSerializer.Deserialize<TExpression>(jsonString, _options);

            Assert.True(ExpressionComparer.AreEqual(expression, deserialized));
        }
    }
}
