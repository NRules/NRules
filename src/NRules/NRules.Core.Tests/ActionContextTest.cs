using System;
using NRules.Core.Rete;
using NUnit.Framework;
using Tuple = NRules.Core.Rete.Tuple;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class ActionContextTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        internal static ActionContext CreateTarget(Tuple tuple)
        {
            return new ActionContext(tuple);
        }

        [Test]
        public void Arg_HasOneObjectOfEachType_ReturnsCorrectObject()
        {
            // Arrange
            var tuple1 = new Tuple(new Fact(new ObjectA()));
            var tuple2 = new Tuple(tuple1, new Fact(new ObjectB()));
            var tuple3 = new Tuple(tuple2, new Fact(new ObjectC()));
            var target = CreateTarget(tuple3);

            // Act
            object a = target.Arg<ObjectA>();
            object b = target.Arg<ObjectB>();
            object c = target.Arg<ObjectC>();

            // Assert
            Assert.True(a.GetType() == typeof(ObjectA));
            Assert.True(b.GetType() == typeof(ObjectB));
            Assert.True(c.GetType() == typeof(ObjectC));
        }

        [Test]
        public void Arg_HasNoObjectOfGivenType_ThrowsException()
        {
            // Arrange
            const string expectedMessage = "Could not get argument of type NRules.Core.Tests.ActionContextTest+ObjectB from action context!";
            var tuple1 = new Tuple(new Fact(new ObjectA()));
            var target = CreateTarget(tuple1);

            // Act - Assert
            string actualMessage = Assert.Throws<ApplicationException>(() => target.Arg<ObjectB>()).Message;
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void Arg_HasMoreThanOneObjectOfGivenType_ThrowsException()
        {
            // Arrange
            const string expectedMessage = "Tuple contained more than one fact of type NRules.Core.Tests.ActionContextTest+ObjectA in action context!";
            var tuple1 = new Tuple(new Fact(new ObjectA()));
            var tuple2 = new Tuple(tuple1, new Fact(new ObjectA()));
            var target = CreateTarget(tuple2);

            // Act - Assert
            string actualMessage = Assert.Throws<ApplicationException>(() => target.Arg<ObjectA>()).Message;
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        private class ObjectA {}
        private class ObjectB {}
        private class ObjectC {}
    }
}
