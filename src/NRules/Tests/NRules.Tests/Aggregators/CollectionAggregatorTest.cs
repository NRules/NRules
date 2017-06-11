using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class CollectionAggregatorTest : AggregatorTest
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
            target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2)));

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
            var result = target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))).ToArray();

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
            var result = target.Add(EmptyTuple(), AsFact(new TestFact[0])).ToArray();

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
            target.Add(EmptyTuple(), AsFact(new TestFact(1)));

            //Act
            var result = target.Add(EmptyTuple(), AsFact(new TestFact(2), new TestFact(3))).ToArray();

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
            target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2)));

            //Act
            var result = target.Modify(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))).ToArray();

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
            target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2)));

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(EmptyTuple(), AsFact(new TestFact(3), new TestFact(4))));
        }

        [Fact]
        public void Remove_ExistingFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2), new TestFact(3)));

            //Act
            var result = target.Remove(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))).ToArray();

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
            target.Add(EmptyTuple(), AsFact(new TestFact(1), new TestFact(2)));

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(EmptyTuple(), AsFact(new TestFact(3), new TestFact(4))));
        }

        private CollectionAggregator<TestFact> CreateTarget()
        {
            return new CollectionAggregator<TestFact>();
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id)
            {
                Id = id;
            }

            public int Id { get; }

            public bool Equals(TestFact other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id == other.Id;
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
                return Id;
            }
        }
    }
}
