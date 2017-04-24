#if NET45 || NET46
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using Xunit;

namespace NRules.Tests
{
    public class ExceptionSerializationTest
    {
        [Fact]
        public void RuleActionEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleActionEvaluationException("Test message", "Test rule", "Test expression", new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.NotNull(newException);
            Assert.NotSame(exception, newException);
            Assert.Equal(exception.Message, newException.Message);
            Assert.Equal(exception.RuleName, newException.RuleName);
            Assert.Equal(exception.Expression, newException.Expression);
            Assert.Equal(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Fact]
        public void RuleConditionEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleConditionEvaluationException("Test message", "Test expression", new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.NotNull(newException);
            Assert.NotSame(exception, newException);
            Assert.Equal(exception.Message, newException.Message);
            Assert.Equal(exception.Expression, newException.Expression);
            Assert.Equal(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Fact]
        public void RuleCompilationEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleCompilationException("Test message", "Test expression", new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.NotNull(newException);
            Assert.NotSame(exception, newException);
            Assert.Equal(exception.Message, newException.Message);
            Assert.Equal(exception.RuleName, newException.RuleName);
            Assert.Equal(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Fact]
        public void RuleDefinitionException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleDefinitionException("Test message", typeof(Rule), new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.NotNull(newException);
            Assert.NotSame(exception, newException);
            Assert.Equal(exception.Message, newException.Message);
            Assert.Equal(exception.RuleType, newException.RuleType);
            Assert.Equal(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Fact]
        public void RuleActivationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleActivationException("Test message", typeof(Rule), new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.NotNull(newException);
            Assert.NotSame(exception, newException);
            Assert.Equal(exception.Message, newException.Message);
            Assert.Equal(exception.RuleType, newException.RuleType);
            Assert.Equal(exception.InnerException.Message, newException.InnerException.Message);
        }

        private T SerializeDeserialize<T>(T originalObject)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, originalObject);
                stream.Seek(0, SeekOrigin.Begin);
                var newObject = (T) formatter.Deserialize(stream);
                return newObject;
            }
        }
    }
}
#endif
