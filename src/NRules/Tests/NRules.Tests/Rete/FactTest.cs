using System;
using NRules.Rete;
using NUnit.Framework;

namespace NRules.Tests.Rete
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
    }
}