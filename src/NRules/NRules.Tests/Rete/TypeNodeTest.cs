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
            var target = new TypeNode(420_69, typeof(DateTime));

            //Act
            var actual = target.FilterType;
            var expected = typeof(DateTime);

            //Assert
            Assert.Equal(expected, actual);
            Assert.Equal(420_69, target.Id);
        }
    }
}