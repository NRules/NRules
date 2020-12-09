using System.Linq;
using NRules.Rete;
using Xunit;

namespace NRules.Tests.Rete
{
    public class TupleTest
    {
        [Fact]
        public void Ctor_WhenFactPassed_ExposedAsRightFact()
        {
            //Arrange
            var fact = new Fact(1);

            //Act
            var target = new Tuple(0, new Tuple(0), fact);

            //Assert
            Assert.Equal(fact, target.RightFact);
        }

        [Fact]
        public void Ctor_WhenTuplePassed_ExposedAsLeftTuple()
        {
            //Arrange
            var tuple1 = new Tuple(0, new Tuple(0), new Fact(1));

            //Act
            var tuple2 = new Tuple(0, tuple1, new Fact(2));

            //Assert
            Assert.Equal(tuple1, tuple2.LeftTuple);
        }

        [Fact]
        public void Enumerator_WhenEnumeratesNTuple_WalksTuplesInReverseOrder()
        {
            //Arrange
            var tuple0 = new Tuple(0);
            var tuple1 = new Tuple(0, tuple0, new Fact(1));
            var tuple2 = new Tuple(0, tuple1, new Fact(2));
            var tuple3 = new Tuple(0, tuple2, new Fact(3));

            //Act
            var target = tuple3.Facts.ToArray();

            //Assert
            Assert.Equal(3, target.Length);
            Assert.Equal(tuple1.RightFact, target[2]);
            Assert.Equal(tuple2.RightFact, target[1]);
            Assert.Equal(tuple3.RightFact, target[0]);
        }

        [Fact]
        public void Enumerator_WhenEnumerated_ReturnsUnderlyingFactObjectsInReverseOrder()
        {
            //Arrange
            var tuple0 = new Tuple(0);
            var tuple1 = new Tuple(0, tuple0, new Fact(1));
            var tuple2 = new Tuple(0, tuple1, new Fact(2));
            var tuple3 = new Tuple(0, tuple2, new Fact(3));

            //Act
            var target = tuple3.Facts.Select(f => f.Value).ToArray();

            //Assert
            Assert.Equal(3, target.Length);
            Assert.Equal(1, target[2]);
            Assert.Equal(2, target[1]);
            Assert.Equal(3, target[0]);
        }

        [Fact]
        public void Enumerator_WhenEnumerates1Tuple_ReturnsSelf()
        {
            //Arrange
            var tuple = new Tuple(0, new Tuple(0), new Fact(1));

            //Act
            var target = tuple.Facts.ToArray();

            //Assert
            Assert.Single(target);
            Assert.Equal(tuple.RightFact, target[0]);
        }

        [Fact]
        public void Enumerator_WhenEnumerates0Tuple_ReturnsEmpty()
        {
            //Arrange
            var tuple = new Tuple(0);

            //Act
            var target = tuple.Facts.ToArray();

            //Assert
            Assert.Empty(target);
        }
    }
}