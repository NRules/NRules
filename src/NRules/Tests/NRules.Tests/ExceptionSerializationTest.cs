using System;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Tests
{
    /*[TestFixture]
    public class ExceptionSerializationTest
    {
        [Test]
        public void RuleActionEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleActionEvaluationException("Test message", "Test rule", "Test expression", new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.IsNotNull(newException);
            Assert.AreNotSame(exception, newException);
            Assert.AreEqual(exception.Message, newException.Message);
            Assert.AreEqual(exception.RuleName, newException.RuleName);
            Assert.AreEqual(exception.Expression, newException.Expression);
            Assert.AreEqual(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Test]
        public void RuleConditionEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleConditionEvaluationException("Test message", "Test expression", new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.IsNotNull(newException);
            Assert.AreNotSame(exception, newException);
            Assert.AreEqual(exception.Message, newException.Message);
            Assert.AreEqual(exception.Expression, newException.Expression);
            Assert.AreEqual(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Test]
        public void RuleCompilationEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleCompilationException("Test message", "Test expression", new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.IsNotNull(newException);
            Assert.AreNotSame(exception, newException);
            Assert.AreEqual(exception.Message, newException.Message);
            Assert.AreEqual(exception.RuleName, newException.RuleName);
            Assert.AreEqual(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Test]
        public void RuleDefinitionException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleDefinitionException("Test message", typeof(Rule), new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.IsNotNull(newException);
            Assert.AreNotSame(exception, newException);
            Assert.AreEqual(exception.Message, newException.Message);
            Assert.AreEqual(exception.RuleType, newException.RuleType);
            Assert.AreEqual(exception.InnerException.Message, newException.InnerException.Message);
        }

        [Test]
        public void RuleActivationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleActivationException("Test message", typeof(Rule), new Exception("Inner exception"));

            //Act
            var newException = SerializeDeserialize(exception);

            //Assert
            Assert.IsNotNull(newException);
            Assert.AreNotSame(exception, newException);
            Assert.AreEqual(exception.Message, newException.Message);
            Assert.AreEqual(exception.RuleType, newException.RuleType);
            Assert.AreEqual(exception.InnerException.Message, newException.InnerException.Message);
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
    }*/
}
