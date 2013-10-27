using System;
using NRules.Rete;
using NUnit.Framework;

namespace NRules.Tests.Rete
{
    [TestFixture]
    public class TypeNodeTest
    {
        [Test]
        public void FilterType_PassedToCtor_Returns()
        {
            //Arrange
            var target = new TypeNode(typeof (DateTime));

            //Act
            var actual = target.FilterType;
            var expected = typeof (DateTime);

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}