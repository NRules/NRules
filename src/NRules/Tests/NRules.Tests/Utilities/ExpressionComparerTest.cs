using System;
using System.Linq.Expressions;
using NRules.Utilities;
using NUnit.Framework;

namespace NRules.Tests.Utilities
{
    [TestFixture]
    public class ExpressionComparerTest
    {
        [Test]
        public void AreEqual_BothNull_True()
        {
            //Arrange - Act
            bool result = ExpressionComparer.AreEqual(null, null);

            //Assert
            Assert.True(result);
        }

        [Test]
        public void AreEqual_EquivalentBinary_True()
        {
            //Arrange
            Expression<Func<int, int, bool>> first = (i1, i2) => i1 == i2;
            Expression<Func<int, int, bool>> second = (ii1, ii2) => ii1 == ii2;

            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void AreEqual_TwoNonEquivalentBinary_False()
        {
            //Arrange
            Expression<Func<int, int, bool>> first = (i1, i2) => i1 == i2;
            Expression<Func<int, int, bool>> second = (ii1, ii2) => ii1 != ii2;

            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.False(result);
        }
        
        [Test]
        public void AreEqual_EquivalentUnary_True()
        {
            //Arrange
            Expression<Func<int, int>> first = i => -i;
            Expression<Func<int, int>> second = i => -i;

            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void AreEqual_EquivalentMember_True()
        {
            //Arrange
            Expression<Func<DateTime, DayOfWeek>> first = d => d.DayOfWeek;
            Expression<Func<DateTime, DayOfWeek>> second = d => d.DayOfWeek;

            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.True(result);
        }

        [Test]
        public void AreEqual_EquivalentMethodCall_True()
        {
            //Arrange
            Expression<Func<DateTime, DateTime>> first = d => d.ToLocalTime();
            Expression<Func<DateTime, DateTime>> second = d => d.ToLocalTime();

            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.True(result);
        }
    }
}