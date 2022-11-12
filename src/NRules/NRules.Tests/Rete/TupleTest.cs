using NRules.Rete;
using Xunit;
using Tuple = NRules.Rete.Tuple;

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
            var target = new Tuple(0, new(0), fact);

            //Assert
            Assert.Equal(fact, target.Fact);
        }

        [Fact]
        public void Ctor_WhenTuplePassed_ExposedAsLeftTuple()
        {
            //Arrange
            var tuple1 = new Tuple(0, new(0), new(1));

            //Act
            var tuple2 = tuple1.CreateChild(0, new(2));

            //Assert
            Assert.Equal(tuple1, tuple2.Parent);
        }

        [Fact]
        public void Count_ShouldBeZero_WhenRootTuple()
        {
            //Arrange
            var tuple = new Tuple(0);

            //Act
            var actual = tuple.Count;

            //Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public void Level_ShouldBeZero_WhenRootTuple()
        {
            //Arrange
            var tuple = new Tuple(0);

            //Act
            var actual = tuple.Level;

            //Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public void Enumerator_WhenEnumeratesNTuple_WalksTuplesInReverseOrder()
        {
            //Arrange
            var tuple0 = new Tuple(0);
            var tuple1 = new Tuple(0, tuple0, new(1));
            var tuple2 = new Tuple(0, tuple1, new(2));
            var tuple3 = new Tuple(0, tuple2, new(3));

            //Act
            var target = tuple3.Facts.ToArray();

            //Assert
            Assert.Equal(3, target.Length);
            Assert.Equal(tuple1.Fact, target[2]);
            Assert.Equal(tuple2.Fact, target[1]);
            Assert.Equal(tuple3.Fact, target[0]);
        }

        [Fact]
        public void Enumerator_WhenEnumeratesNTuple_WalksTuplesInSameOrderAsFacts()
        {
            //Arrange
            var tuple0 = new Tuple(0);
            var tuple1 = new Tuple(0, tuple0, new(1));
            var tuple2 = new Tuple(0, tuple1);
            var tuple3 = new Tuple(0, tuple2);
            var tuple4 = new Tuple(0, tuple3, new(3));
            var tuple5 = new Tuple(0, tuple4);
            var expected = tuple5.Facts.ToArray();

            //Act
            var actual = new List<Fact>();
            foreach (var fact in tuple5)
            {
                actual.Add(fact);
            }

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Enumerator_WhenEnumerated_ReturnsUnderlyingFactObjectsInReverseOrder()
        {
            //Arrange
            var tuple0 = new Tuple(0);
            var tuple1 = new Tuple(0, tuple0, new(1));
            var tuple2 = new Tuple(0, tuple1, new(2));
            var tuple3 = new Tuple(0, tuple2, new(3));

            //Act
            var target = tuple3.Facts.Select(f => f.Object).ToArray();

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
            var tuple = new Tuple(0, new(0), new(1));

            //Act
            var target = tuple.Facts.ToArray();

            //Assert
            Assert.Single(target);
            Assert.Equal(tuple.Fact, target[0]);
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