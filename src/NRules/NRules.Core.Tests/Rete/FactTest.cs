using System;
using NRules.Core.Rete;
using NUnit.Framework;

namespace NRules.Core.Tests.Rete
{
    [TestFixture]
    public class FactTest
    {
        [Test]
        public void FactType_WhenObjectPassed_ReturnsItsType()
        {
            //Arrange
            var obj = new DateTime();
            var target = new Fact(obj);

            //Act
            var actual = target.FactType;
            var expected = typeof (DateTime);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Equality_WhenFactsContainSameObject_ReturnsTrue()
        {
            //Arrange
            var obj = DateTime.Now;
            var target1 = new Fact(obj);
            var target2 = new Fact(obj);

            //Act
            var actual = target1.Equals(target2);
            var expected = true;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Equality_WhenFactsContainEqualObjects_ReturnsTrue()
        {
            //Arrange
            var obj1 = "Some Value";
            var obj2 = "Some Value";
            var target1 = new Fact(obj1);
            var target2 = new Fact(obj2);

            //Act
            var actual = target1.Equals(target2);
            var expected = true;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Equality_WhenFactsContainDifferentObjects_ReturnsFalse()
        {
            //Arrange
            var obj1 = "Some Value";
            var obj2 = "Another Value";
            var target1 = new Fact(obj1);
            var target2 = new Fact(obj2);

            //Act
            var actual = target1.Equals(target2);
            var expected = false;

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}