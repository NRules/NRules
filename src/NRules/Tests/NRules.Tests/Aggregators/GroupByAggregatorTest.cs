using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.RuleModel.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class GroupByAggregatorTest
    {
        [Fact]
        public void Aggregates_NewInstance_Empty()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Aggregates_HasGroups_AllGroups()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1"), new TestFact(3, "key2"), new TestFact(4, null)});

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.Equal(3, result.Length);
        }

        [Fact]
        public void Add_NewGroupNewInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1"), new TestFact(3, "key2")}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Add_NewGroupHasDefaultKey_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, null)}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal(null, aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Add_NewGroupExistingInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Add(new[] {new TestFact(3, "key2"), new TestFact(4, "key2")}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key2", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
        }

        [Fact]
        public void Add_ExistingGroup_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key2")});

            //Act
            var result = target.Add(new[] {new TestFact(3, "key1"), new TestFact(4, "key2")}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Add_ExistingGroupHasDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, null)});

            //Act
            var result = target.Add(new[] {new TestFact(3, "key1"), new TestFact(4, null)}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Modified, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal(null, aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Add_NewAndExistingGroups_AddedAndModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1")});

            //Act
            var result = target.Add(new[] {new TestFact(2, "key1"), new TestFact(3, "key2"), new TestFact(4, "key2")}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(2, aggregate2.Count());
        }

        [Fact]
        public void Add_EmptyGroup_NoResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new TestFact[0]).ToArray();

            //Assert
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public void Modify_ExistingGroups_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Modify(new[] {new TestFact(1, "key1")}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_ExistingGroupsHasDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, null), new TestFact(2, null) });

            //Act
            var result = target.Modify(new[] {new TestFact(1, null)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_GroupRemovedAndAdded_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key2"), new TestFact(3, "key2")});

            //Act
            var result = target.Modify(new[] {new TestFact(1, "key2"), new TestFact(2, "key1")}).ToArray();

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
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Modify(new[] {new TestFact(2, "key2")}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal("key2", aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Modify_ExistingGroupKeyChangedToDefault_ModifiedAndAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Modify(new[] {new TestFact(2, null)}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
            Assert.Equal(null, aggregate2.Key);
            Assert.Equal(1, aggregate2.Count());
        }

        [Fact]
        public void Modify_ExistingGroupAllElementsHaveKeyChanged_RemovedAndAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Modify(new[] {new TestFact(1, "key2"), new TestFact(2, "key2")}).ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(0, aggregate1.Count());
            Assert.Equal(AggregationAction.Added, result[1].Action);
            var aggregate2 = (IGrouping<string, TestFact>)result[1].Aggregate;
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
                () => target.Modify(new[] {new TestFact(1, "key1"), new TestFact(2, "key2")}));
        }

        [Fact]
        public void Modify_NonExistentDefaultKey_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(new[] {new TestFact(1, null)}));
        }

        [Fact]
        public void Remove_ExistingGroup_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Remove(new[] {new TestFact(1, "key1")}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
        }

        [Fact]
        public void Remove_ExistingGroupWithDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, null), new TestFact(2, null)});

            //Act
            var result = target.Remove(new[] {new TestFact(1, null)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal(null, aggregate1.Key);
            Assert.Equal(1, aggregate1.Count());
        }

        [Fact]
        public void Remove_ExistingGroupAllElementsRemoved_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")});

            //Act
            var result = target.Remove(new[] {new TestFact(1, "key1"), new TestFact(2, "key1")}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
            Assert.Equal("key1", aggregate1.Key);
            Assert.Equal(0, aggregate1.Count());
        }

        [Fact]
        public void Remove_ExistingGroupAllElementsRemovedDefaultKey_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, null), new TestFact(2, null)});

            //Act
            var result = target.Remove(new[] {new TestFact(1, null), new TestFact(2, null)}).ToArray();

            //Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(AggregationAction.Removed, result[0].Action);
            var aggregate1 = (IGrouping<string, TestFact>)result[0].Aggregate;
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
                () => target.Remove(new[] {new TestFact(1, "key1"), new TestFact(2, "key2")}));
        }

        [Fact]
        public void Remove_NonExistentDefaultKey_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(new[] {new TestFact(1, null)}));
        }

        private GroupByAggregator<TestFact, string, TestFact> CreateTarget()
        {
            return new GroupByAggregator<TestFact, string, TestFact>(x => x.Key, x => x);
        }

        private class TestFact : IEquatable<TestFact>
        {
            private readonly int _id;
            private readonly string _key;

            public TestFact(int id, string key)
            {
                _id = id;
                _key = key;
            }

            public int Id { get { return _id; } }
            public string Key { get { return _key; } }

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