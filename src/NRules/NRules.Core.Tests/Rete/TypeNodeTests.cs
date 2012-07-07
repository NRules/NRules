using System;
using NRules.Core.Rete;
using NUnit.Framework;

namespace NRules.Core.Tests.Rete
{
    [TestFixture]
    public class TypeNodeTests
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