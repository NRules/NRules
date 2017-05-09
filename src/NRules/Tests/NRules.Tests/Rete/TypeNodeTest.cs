using System;
using NRules.Rete;
using Xunit;

namespace NRules.Tests.Rete
{
    public class TypeNodeTest
    {
        [Fact]
        public void FilterType_PassedToCtor_Returns()
        {
            //Arrange
            var target = new TypeNode(typeof (DateTime));

            //Act
            var actual = target.FilterType.AsType();
            var expected = typeof (DateTime);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}