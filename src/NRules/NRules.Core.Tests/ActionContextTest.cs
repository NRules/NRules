using System;
using NRules.Core.Rete;
using NUnit.Framework;
using Tuple = NRules.Core.Rete.Tuple;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class ActionContextTest
    {
        internal static ActionContext CreateTarget(Tuple tuple)
        {
            return new ActionContext(null, null, tuple);
        }

        [Test]
        public void Arg_HasOneObjectOfEachType_ReturnsCorrectObject()
        {
            // Arrange
            var tuple1 = new Tuple(new Tuple(), new Fact(new ObjectA()));
            var tuple2 = new Tuple(tuple1, new Fact(new ObjectB()));
            var tuple3 = new Tuple(tuple2, new Fact(new ObjectC()));
            var target = CreateTarget(tuple3);

            // Act
            object a = target.Arg<ObjectA>();
            object b = target.Arg<ObjectB>();
            object c = target.Arg<ObjectC>();

            // Assert
            Assert.True(a.GetType() == typeof (ObjectA));
            Assert.True(b.GetType() == typeof (ObjectB));
            Assert.True(c.GetType() == typeof (ObjectC));
        }

        [Test]
        public void Arg_HasNoObjectOfGivenType_ThrowsException()
        {
            // Arrange
            var tuple1 = new Tuple(new Tuple(), new Fact(new ObjectA()));
            var target = CreateTarget(tuple1);

            // Act - Assert
            Assert.Throws<InvalidOperationException>(() => target.Arg<ObjectB>());
        }

        [Test]
        public void Arg_HasMoreThanOneObjectOfGivenType_ReturnsFirst()
        {
            // Arrange
            var tuple1 = new Tuple(new Tuple(), new Fact(new ObjectA()));
            var tuple2 = new Tuple(tuple1, new Fact(new ObjectA()));
            var target = CreateTarget(tuple2);

            // Act
            var a = target.Arg<ObjectA>();

            // Assert
            Assert.AreSame(tuple1.RightFact.Object, a);
        }

        private class ObjectA
        {
        }

        private class ObjectB
        {
        }

        private class ObjectC
        {
        }
    }
}