using System.Linq;
using NRules.Rete;
using NUnit.Framework;

namespace NRules.Tests.Rete
{
    [TestFixture]
    public class TupleTest
    {
        [Test]
        public void Ctor_WhenFactPassed_ExposedAsRightFactAndChained()
        {
            //Arrange
            var fact = new Fact(1);

            //Act
            var target = new Tuple(new Tuple(), fact);

            //Assert
            Assert.AreEqual(fact, target.RightFact);
            Assert.Contains(target, fact.ChildTuples.ToArray());
        }

        [Test]
        public void Ctor_WhenTuplePassed_ExposedAsLeftTupleAndChained()
        {
            //Arrange
            var tuple1 = new Tuple(new Tuple(), new Fact(1));

            //Act
            var tuple2 = new Tuple(tuple1, new Fact(2));

            //Assert
            Assert.AreEqual(tuple1, tuple2.LeftTuple);
            Assert.Contains(tuple2, tuple1.ChildTuples.ToArray());
        }

        [Test]
        public void Enumerator_WhenEnumeratesNTuple_WalksTuplesInOrder()
        {
            //Arrange
            var tuple0 = new Tuple();
            var tuple1 = new Tuple(tuple0, new Fact(1));
            var tuple2 = new Tuple(tuple1, new Fact(2));
            var tuple3 = new Tuple(tuple2, new Fact(3));

            //Act
            var target = tuple3.ToArray();

            //Assert
            Assert.AreEqual(3, target.Length);
            Assert.AreEqual(tuple1.RightFact, target[0]);
            Assert.AreEqual(tuple2.RightFact, target[1]);
            Assert.AreEqual(tuple3.RightFact, target[2]);
        }

        [Test]
        public void Enumerator_WhenGetFactObjects_ReturnsUnderlyingFactObjectsInOrder()
        {
            //Arrange
            var tuple0 = new Tuple();
            var tuple1 = new Tuple(tuple0, new Fact(1));
            var tuple2 = new Tuple(tuple1, new Fact(2));
            var tuple3 = new Tuple(tuple2, new Fact(3));

            //Act
            var target = tuple3.GetFactObjects();

            //Assert
            Assert.AreEqual(3, target.Length);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
        }

        [Test]
        public void Enumerator_WhenEnumerates1Tuple_ReturnsSelf()
        {
            //Arrange
            var tuple = new Tuple(new Tuple(), new Fact(1));

            //Act
            var target = tuple.ToArray();

            //Assert
            Assert.AreEqual(1, target.Length);
            Assert.AreEqual(tuple.RightFact, target[0]);
        }

        [Test]
        public void Clear_WhenCalledOn1Tuple_ClearsItselfAndUnchainsFact()
        {
            //Arrange
            var fact = new Fact(1);
            var target = new Tuple(new Tuple(), fact);

            //Act
            target.Clear();

            //Assert
            Assert.IsNull(target.RightFact);
            Assert.AreEqual(0, fact.ChildTuples.Count);
        }

        [Test]
        public void Clear_WhenCalledOn2Tuple_ClearsItselfAndUnchainsFactAndUnchainsTuple()
        {
            //Arrange
            var tuple = new Tuple(new Tuple(), new Fact(1));
            var fact = new Fact(2);
            var target = new Tuple(tuple, fact);

            //Act
            target.Clear();

            //Assert
            Assert.IsNull(target.RightFact);
            Assert.IsNull(target.LeftTuple);
            Assert.AreEqual(0, fact.ChildTuples.Count);
            Assert.AreEqual(0, tuple.ChildTuples.Count);
        }
    }
}