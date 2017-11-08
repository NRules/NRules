using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class GroupByAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_NewGroupNewInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"), new TestFact(3, "key2"));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Add_NewGroupHasDefaultKey_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, null));
            var result = target.Add(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal(null, aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Add_NewGroupExistingInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key2"), new TestFact(4, "key2"));
            var result = target.Add(EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key2", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
        }

        [Fact]
        public void Add_ExistingGroup_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"));
            target.Add(EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key1"), new TestFact(4, "key2"));
            var result = target.Add(EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Add_ExistingGroupHasDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, null));
            target.Add(EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key1"), new TestFact(4, null));
            var result = target.Add(EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal(null, aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Add_NewAndExistingGroups_AddedAndModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(2, "key1"), new TestFact(3, "key2"), new TestFact(4, "key2"));
            var result = target.Add(EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Add_EmptyGroup_NoResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(EmptyTuple(), AsFact(new TestFact[0])).ToArray();

            //Assert
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Modify_ExistingGroupsSameIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key1");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_ExistingGroupsDifferentIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(3, "key1");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_ExistingGroupsHasDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, null);
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_GroupRemovedAndAdded_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"), new TestFact(3, "key2"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key2");
            facts[1].Value = new TestFact(2, "key1");
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            Assert.Equal(AggregationAction.Modified, result[1].Action);
        }

        [Fact]
        public void Modify_ExistingGroupKeyChanged_ModifiedAndAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, "key2");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Modify_ExistingGroupKeyChangedToDefault_ModifiedAndAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, null);
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal(null, aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Modify_ExistingGroupAllElementsHaveKeyChanged_RemovedAndAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key2");
            facts[1].Value = new TestFact(2, "key2");
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(0, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, GroupElement>) result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(EmptyTuple(), AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"))));
        }

        [Fact]
        public void Modify_NonExistentDefaultKey_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(EmptyTuple(), AsFact(new TestFact(1, null))));
        }

        [Fact]
        public void Remove_ExistingGroup_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key1");
            var toRemove = facts.Take(1).ToArray();
            var result = target.Remove(EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
        }

        [Fact]
        public void Remove_ExistingGroupWithDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
            target.Add(EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, null);
            var toRemove = facts.Take(1).ToArray();
            var result = target.Remove(EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal(null, aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
        }

        [Fact]
        public void Remove_ExistingGroupAllElementsRemoved_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(EmptyTuple(), facts);

            //Act
            var result = target.Remove(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(0, aggregate1.Count());
        }

        [Fact]
        public void Remove_ExistingGroupAllElementsRemovedDefaultKey_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
            target.Add(EmptyTuple(), facts);

            //Act
            var result = target.Remove(EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            var aggregate1 = (IGrouping<string, GroupElement>) result[0].Aggregate;
            Assert.Equal(null, aggregate1.Key);
            Assert.Equal(0, aggregate1.Count());
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(EmptyTuple(), AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"))));
        }

        [Fact]
        public void Remove_NonExistentDefaultKey_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(EmptyTuple(), AsFact(new TestFact(1, null))));
        }

        private GroupByAggregator<TestFact, string, GroupElement> CreateTarget()
        {
            var keyExpression = new FactExpression<TestFact, string>(x => x.Key);
            var elementExpression = new FactExpression<TestFact, GroupElement>(x => new GroupElement(x));
            return new GroupByAggregator<TestFact, string, GroupElement>(keyExpression, elementExpression);
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id, string key)
            {
                Id = id;
                Key = key;
            }

            public int Id { get; }
            public string Key { get; }

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

        private class GroupElement
        {
            public GroupElement(TestFact fact)
            {
                Id = fact.Id;
                Key = fact.Key;
            }

            public int Id { get; }
            public string Key { get; }
        }
    }
}