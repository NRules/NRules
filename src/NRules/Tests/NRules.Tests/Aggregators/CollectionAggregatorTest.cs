using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.RuleModel.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class CollectionAggregatorTest
    {
        [Fact]
        public void Aggregates_NewInstance_OneEmptyCollection()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            var aggregate = (IEnumerable<TestFact>)result[0];
            Assert.Equal(0, aggregate.Count());
        }

        [Fact]
        public void Aggregates_HasElements_OneCollectionWithElements()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            var aggregate = (IEnumerable<TestFact>)result[0];
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Add_NewInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new[] {new TestFact(1), new TestFact(2)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Add_NewInstanceNoFacts_AddedResultEmptyCollection()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new TestFact[0]).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.Equal(0, aggregate.Count());
        }

        [Fact]
        public void Add_OldInstance_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1)});

            //Act
            var result = target.Add(new[] {new TestFact(2), new TestFact(3)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.Equal(3, aggregate.Count());
        }

        [Fact]
        public void Modify_ExistingFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act
            var result = target.Modify(new[] {new TestFact(1), new TestFact(2)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2)});

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(new[] {new TestFact(3), new TestFact(4)}));
        }

        [Fact]
        public void Remove_ExistingFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1), new TestFact(2), new TestFact(3)});

            //Act
            var result = target.Remove(new[] {new TestFact(1), new TestFact(2)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate = (IEnumerable<TestFact>)result[0].Aggregate;
            Assert.Equal(1, aggregate.Count());
            Assert.Equal(3, aggregate.ElementAt(0).Id);
        }

        [Fact]
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
