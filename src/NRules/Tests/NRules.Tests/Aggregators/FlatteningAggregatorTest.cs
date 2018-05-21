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
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

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
            var facts = AsFact(new TestFact(1, "value11", "value12", "value12", "valuex"), new TestFact(2, "value21", "value21", "value22", "valuex"));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(5, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Added, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
            Assert.Equal(AggregationAction.Added, result[2].Action);
            Assert.Equal("valuex", result[2].Aggregate);
            Assert.Equal(AggregationAction.Added, result[3].Action);
            Assert.Equal("value21", result[3].Aggregate);
            Assert.Equal(AggregationAction.Added, result[4].Action);
            Assert.Equal("value22", result[4].Aggregate);
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
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Modify_ExistingFactsSameIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

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
            target.Add(null, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(3, "value31", "value32");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

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
            target.Add(null, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, "value12", "value13");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

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
        public void Modify_FactsWithDuplicates_CorrectResult()
        {
            //Arrange
            var target = CreateTarget();

            var facts = AsFact(new TestFact(1, "value11", "value12", "value12", "valuex"), new TestFact(2, "value21", "value21", "value22", "valuex"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, "value12", "value13");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null, EmptyTuple(), toUpdate).ToArray();

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
                () => target.Modify(null, EmptyTuple(), AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"))));
        }

        [Fact]
        public void Remove_ExistingFacts_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"));
            target.Add(null, EmptyTuple(), facts);

            //Act
            var toRemove = facts.Take(1).ToArray();
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            Assert.Equal("value11", result[0].Aggregate);
            Assert.Equal(AggregationAction.Removed, result[1].Action);
            Assert.Equal("value12", result[1].Aggregate);
        }

        [Fact]
        public void Remove_FactsWithDuplicates_CorrectResult()
        {
            //Arrange
            var target = CreateTarget();

            var facts = AsFact(new TestFact(1, "value11", "value12", "value12", "valuex"), new TestFact(2, "value21", "value21", "value22", "valuex"));
            target.Add(null, EmptyTuple(), facts);

            //Act - I
            var toRemove1 = facts.Take(1).ToArray();
            var result1 = target.Remove(null, EmptyTuple(), toRemove1).ToArray();

            //Assert - I
            Assert.Equal(2, result1.Length);
            Assert.Equal(AggregationAction.Removed, result1[0].Action);
            Assert.Equal("value11", result1[0].Aggregate);
            Assert.Equal(AggregationAction.Removed, result1[1].Action);
            Assert.Equal("value12", result1[1].Aggregate);

            //Act - II
            var toRemove2 = facts.Skip(1).Take(1).ToArray();
            var result2 = target.Remove(null, EmptyTuple(), toRemove2).ToArray();

            //Assert - II
            Assert.Equal(3, result2.Length);
            Assert.Equal(AggregationAction.Removed, result2[0].Action);
            Assert.Equal("value21", result2[0].Aggregate);
            Assert.Equal(AggregationAction.Removed, result2[1].Action);
            Assert.Equal("value22", result2[1].Aggregate);
            Assert.Equal(AggregationAction.Removed, result2[2].Action);
            Assert.Equal("valuex", result2[2].Aggregate);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(null, EmptyTuple(), AsFact(new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22"))));
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