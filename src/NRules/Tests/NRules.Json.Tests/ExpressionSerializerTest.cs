using System;
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
        public void Roundtrip_PropertyAccess_Equals()
        {
            //Arrange
            Expression<Func<string, int>> original = s => s.Length;

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(ExpressionComparer.AreEqual(original, deserialized));
        }

        [Fact]
        public void Roundtrip_InstanceMethod_Equals()
        {
            //Arrange
            Expression<Func<string, string>> original = s => s.ToUpper();

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(ExpressionComparer.AreEqual(original, deserialized));
        }

        [Fact]
        public void Roundtrip_StaticMethod_Equals()
        {
            //Arrange
            Expression<Func<string, string>> original = s => string.Concat(s, s);

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(ExpressionComparer.AreEqual(original, deserialized));
        }

        [Fact]
        public void Roundtrip_ExtensionMethod_Equals()
        {
            //Arrange
            Expression<Func<string, string>> original = s => s.Transform();

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(ExpressionComparer.AreEqual(original, deserialized));
        }

        [Fact]
        public void Roundtrip_BinaryExpressionEquals_Equals()
        {
            //Arrange
            Expression<Func<string, string, bool>> original = (s1, s2) => s1 == s2;

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(ExpressionComparer.AreEqual(original, deserialized));
        }

        private TExpression Roundtrip<TExpression>(TExpression original)
        {
            var jsonString = JsonSerializer.Serialize(original, _options);
            //System.IO.File.WriteAllText(@"C:\temp\expression.json", jsonString);
            var deserialized = JsonSerializer.Deserialize<TExpression>(jsonString, _options);
            return deserialized;
        }
    }
}
