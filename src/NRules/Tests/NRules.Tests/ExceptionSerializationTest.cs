using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace NRules.Tests
{
    [TestFixture]
    public class ExceptionSerializationTest
    {
        private IFormatter _formatter;
        private Stream _stream;

        [SetUp]
        public void SetUp()
        {
            _formatter = new BinaryFormatter();
            _stream = new MemoryStream();
        }

        [TearDown]
        public void TearDown()
        {
            _stream.Close();
        }

        [Test]
        public void RuleActionEvaluationException_SerializedDeserialized_Equals()
        {
            //Arrange
            var exception = new RuleActionEvaluationException("Test message", "Test expression", new Exception("Inner exception"));

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

        private T SerializeDeserialize<T>(T originalObject)
        {
            _formatter.Serialize(_stream, originalObject);
            _stream.Seek(0, SeekOrigin.Begin);
            var newObject = (T) _formatter.Deserialize(_stream);
            return newObject;
        }
    }
}
