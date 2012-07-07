using System;
using NRules.Core.Rete;
using NUnit.Framework;

namespace NRules.Core.Tests.Rete
{
    [TestFixture]
    public class FactTests
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