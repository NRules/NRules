using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class ProjectionAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_Facts_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            Assert.Equal("value1", result[0].Aggregate);
            Assert.Equal(AggregationAction.Added, result[1].Action);
            Assert.Equal("value2", result[1].Aggregate);
        }

        [Fact]
        public void Add_NoFacts_EmptyResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact[0]);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Modify_Existing_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            Assert.Equal("value1", result[0].Aggregate);
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            Assert.Equal("value2", result[1].Aggregate);
        }

        [Fact]
        public void Modify_ProjectionChangedSameIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "value1x");
            facts[1].Value = new TestFact(2, "value2x");
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            Assert.Equal("value1", result[0].Previous);
            Assert.Equal("value1x", result[0].Aggregate);
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            Assert.Equal("value2", result[1].Previous);
            Assert.Equal("value2x", result[1].Aggregate);
        }

        [Fact]
        public void Modify_ProjectionChangedDifferentIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(4, "value4x");
            facts[1].Value = new TestFact(5, "value5x");
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            Assert.Equal("value1", result[0].Previous);
            Assert.Equal("value4x", result[0].Aggregate);
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            Assert.Equal("value2", result[1].Previous);
            Assert.Equal("value5x", result[1].Aggregate);
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(null, EmptyTuple(), AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"))));
        }

        [Fact]
        public void Remove_Existing_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            Assert.Equal("value1", result[0].Aggregate);
            Assert.Equal(AggregationAction.Removed, result[1].Action);
            Assert.Equal("value2", result[1].Aggregate);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(null, EmptyTuple(), AsFact(new TestFact(1, "value1"), new TestFact(2, "value2"))));
        }

        private ProjectionAggregator<TestFact, string> CreateTarget()
        {
            var expression = new FactExpression<TestFact, string>(x => x.Value);
            return new ProjectionAggregator<TestFact, string>(expression);
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id, string value)
            {
                Id = id;
                Value = value;
            }

            public int Id { get; }
            public string Value { get; }

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
                return Equals((TestFact) obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }
    }
}