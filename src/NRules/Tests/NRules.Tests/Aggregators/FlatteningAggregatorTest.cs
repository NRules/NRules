using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class FlatteningAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_Facts_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(4, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Added, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
            Assert.Equal(AggregationAction.Added, result[2].Action);
            Assert.Equal("value21", result[2].Aggregate);
            Assert.Equal(AggregationAction.Added, result[3].Action);
            Assert.Equal("value22", result[3].Aggregate);
        }

        [Fact]
        public void Add_FactsWithDuplicates_AddedDistinctResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "value11", "value12", "value12"), new TestFact(2, "value21", "value21", "value22"));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(4, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Added, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
            Assert.Equal(AggregationAction.Added, result[2].Action);
            Assert.Equal("value21", result[2].Aggregate);
            Assert.Equal(AggregationAction.Added, result[3].Action);
            Assert.Equal("value22", result[3].Aggregate);
        }

        [Fact]
        public void Add_NoFacts_EmptyResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact[0]);
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Modify_ExistingFactsSameIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            target.Add(EmptyTuple(), facts);

            //Act
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
        }

        [Fact]
        public void Modify_ExistingFactsDifferentIdentity_RemovedAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(3, "value31", "value32");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(4, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Removed, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
            Assert.Equal(AggregationAction.Added, result[2].Action);
            Assert.Equal("value31", result[2].Aggregate);
            Assert.Equal(AggregationAction.Added, result[3].Action);
            Assert.Equal("value32", result[3].Aggregate);
        }

        [Fact]
        public void Modify_ExistingFactsHasAdditionsModificationsAndRemovals_CorrectResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, "value12", "value13");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(3, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
            Assert.Equal(AggregationAction.Added, result[2].Action);
            Assert.Equal("value13", result[2].Aggregate);
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(EmptyTuple(), AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"))));
        }

        [Fact]
        public void Remove_ExistingFacts_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            target.Add(EmptyTuple(), facts);

            //Act
            var toRemove = facts.Take(1).ToArray();
            var result = target.Remove(EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Removed, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(EmptyTuple(), AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"))));
        }

        private FlatteningAggregator<TestFact, string> CreateTarget()
        {
            var expression = new FactExpression<TestFact, IEnumerable<string>>(x => x.Values);
            return new FlatteningAggregator<TestFact, string>(expression);
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id, params string[] values)
            {
                Id = id;
                Values = values;
            }

            public int Id { get; }
            public string[] Values { get; }

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