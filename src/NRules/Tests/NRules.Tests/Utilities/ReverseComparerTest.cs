using System.Collections.Generic;
using NRules.Utilities;
using Xunit;

namespace NRules.Tests.Utilities
{
    public class ReverseComparerTest
    {
        [Fact]
        public void ReverseDefaultIntegerComparer_LessThan()
        {
            // Arrange
            var comparer = Comparer<int>.Default;
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(-1, comparer.Compare(1, 2));

            // Act-Assert
            Assert.Equal(1, reversedComparer.Compare(1, 2));
        }

        [Fact]
        public void ReverseDefaultIntegerComparer_Equal()
        {
            // Arrange
            var comparer = Comparer<int>.Default;
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(0, comparer.Compare(1, 1));

            // Act-Assert
            Assert.Equal(0, reversedComparer.Compare(1, 1));
        }

        [Fact]
        public void ReverseDefaultIntegerComparer_GreaterThan()
        {
            // Arrange
            var comparer = Comparer<int>.Default;
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(1, comparer.Compare(2, 1));

            // Act-Assert
            Assert.Equal(-1, reversedComparer.Compare(2, 1));
        }

        [Fact]
        public void ReverseDefaultStringComparer_WithNull_LessThan()
        {
            // Arrange
            var comparer = Comparer<string>.Default;
            var reversedComparer = new ReverseComparer<string>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(-1, comparer.Compare(null, "A"));

            // Act-Assert
            Assert.Equal(1, reversedComparer.Compare(null, "A"));
        }

        [Fact]
        public void ReverseDefaultStringComparer_WithNull_LessEqual()
        {
            // Arrange
            var comparer = Comparer<string>.Default;
            var reversedComparer = new ReverseComparer<string>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(0, comparer.Compare(null, null));

            // Act-Assert
            Assert.Equal(0, reversedComparer.Compare(null, null));
        }

        [Fact]
        public void ReverseDefaultStringComparer_WithNull_GreaterThan()
        {
            // Arrange
            var comparer = Comparer<string>.Default;
            var reversedComparer = new ReverseComparer<string>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(1, comparer.Compare("A", null));

            // Act-Assert
            Assert.Equal(-1, reversedComparer.Compare("A", null));
        }

        [Fact]
        public void CustomComparer_LessThan()
        {
            // Arrange
            var comparer = new CustomIntComparer();
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(-2, comparer.Compare(1, 2));

            // Act-Assert
            Assert.Equal(2, reversedComparer.Compare(1, 2));
        }

        [Fact]
        public void CustomComparer_Equal()
        {
            // Arrange
            var comparer = new CustomIntComparer();
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(0, comparer.Compare(1, 1));

            // Act-Assert
            Assert.Equal(0, reversedComparer.Compare(1, 1));
        }

        [Fact]
        public void CustomComparer_GreaterThan()
        {
            // Arrange
            var comparer = new CustomIntComparer();
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(2, comparer.Compare(2, 1));

            // Act-Assert
            Assert.Equal(-2, reversedComparer.Compare(2, 1));
        }

        [Fact]
        public void CustomComparer_Limits_LessThan()
        {
            // Arrange
            var comparer = new CustomIntComparerLimits();
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(int.MinValue, comparer.Compare(1, 2));

            // Act-Assert
            Assert.Equal(int.MaxValue, reversedComparer.Compare(1, 2));
        }

        [Fact]
        public void CustomComparer_Limits_GreaterThan()
        {
            // Arrange
            var comparer = new CustomIntComparerLimits();
            var reversedComparer = new ReverseComparer<int>(comparer);

            // Act-Assert (Precondition)
            Assert.Equal(int.MaxValue, comparer.Compare(2, 1));

            // Act-Assert
            Assert.Equal(int.MinValue, reversedComparer.Compare(2, 1));
        }

        class CustomIntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                var result = 0;

                if (x != y)
                {
                    result = x > y ? 2 : -2;
                }

                return result;
            }
        }

        class CustomIntComparerLimits : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                var result = 0;

                if (x != y)
                {
                    result = x > y ? int.MaxValue : int.MinValue;
                }

                return result;
            }
        }
    }
}