using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.RuleModel.Aggregators;
using NUnit.Framework;

namespace NRules.Tests.Aggregators
{
    [TestFixture]
    public class CollectionAggregatorTest
    {
        [Test]
        public void Aggregates_NewInstance_OneEmptyCollection()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            var aggregate = (IEnumerable<TestFact>)result[0];
            Assert.AreEqual(0, aggregate.Count());
        }

        [Test]
        public void Aggregates_HasElements_OneCollectionWithElements()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            var aggregate = (IEnumerable<TestFact>)result[0];
            Assert.AreEqual(2, aggregate.Count());
        }

        [Test]
        public void Add_NewInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new[] {new TestFact(1), new TestFact(2)}).ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.AreEqual(2, aggregate.Count());
        }

        [Test]
        public void Add_NewInstanceNoFacts_AddedResultEmptyCollection()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new TestFact[0]).ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.AreEqual(0, aggregate.Count());
        }

        [Test]
        public void Add_OldInstance_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1)});

            //Act
            var result = target.Add(new[] {new TestFact(2), new TestFact(3)}).ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.AreEqual(3, aggregate.Count());
        }

        [Test]
        public void Modify_ExistingFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act
            var result = target.Modify(new[] {new TestFact(1), new TestFact(2)}).ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.AreEqual(2, aggregate.Count());
        }

        [Test]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(new[] {new TestFact(3), new TestFact(4)}));
        }

        [Test]
        public void Remove_ExistingFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2), new TestFact(3)});

            //Act
            var result = target.Remove(new[] {new TestFact(1), new TestFact(2)}).ToArray();

            //Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.AreEqual(1, aggregate.Count());
            Assert.AreEqual(3, aggregate.ElementAt(0).Id);
        }

        [Test]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(new[] {new TestFact(3), new TestFact(4)}));
        }

        private CollectionAggregator<TestFact> CreateTarget()
        {
            return new CollectionAggregator<TestFact>();
        }

        private class TestFact : IEquatable<TestFact>
        {
            private readonly int _id;

            public TestFact(int id)
            {
                _id = id;
            }

            public int Id { get { return _id; } }

            public bool Equals(TestFact other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _id == other._id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TestFact)obj);
            }

            public override int GetHashCode()
            {
                return _id;
            }
        }
    }
}
