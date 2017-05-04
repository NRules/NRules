using System;
using NRules.Rete;
using Xunit;

namespace NRules.Tests.Rete
{
    public class FactTest
    {
        [Fact]
        public void FactType_WhenObjectPassed_ReturnsItsType()
        {
            //Arrange
            var obj = new DateTime();
            var target = new Fact(obj);

            //Act
            var actual = target.FactType.AsType();
            var expected = typeof (DateTime);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}