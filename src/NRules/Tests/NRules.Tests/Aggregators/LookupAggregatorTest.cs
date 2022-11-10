using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class LookupAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_NoFacts_AddedResultEmptyLookupNonExistentKeysEmpty()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(Array.Empty<TestFact>());
            var result = target.Add(null!, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Empty(aggregate);
            Assert.Empty(aggregate[GetKey("key1")]);
            Assert.Empty(aggregate[GetKey(null)]);
        }

        [Fact]
        public void Add_NewGroupNewInstance_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"), new TestFact(3, "key2"));
            var result = target.Add(null!, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
            Assert.Equal(2, aggregate[GetKey("key1")].Count());
            Assert.Single(aggregate[GetKey("key2")]);
        }

        [Fact]
        public void Add_NewGroupHasDefaultKey_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, null));
            var result = target.Add(null!, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Added, result[0].Action);
            var aggregate = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate.Count());
            Assert.Single(aggregate[GetKey("key1")]);
            Assert.Single(aggregate[GetKey(null)]);
        }

        [Fact]
        public void Add_NewGroupExistingInstance_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key2"), new TestFact(4, "key2"));
            var result = target.Add(null!, EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(2, aggregate1[GetKey("key1")].Count());
            Assert.Equal(2, aggregate1[GetKey("key2")].Count());
        }

        [Fact]
        public void Add_ExistingGroup_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key1"), new TestFact(4, "key2"));
            var result = target.Add(null!, EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(2, aggregate1[GetKey("key1")].Count());
            Assert.Equal(2, aggregate1[GetKey("key2")].Count());
        }

        [Fact]
        public void Add_KeyPayloadChanges_KeyPayloadUpdated()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1") { Payload = 1 }, new TestFact(2, "key2") { Payload = 1 });
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key1") { Payload = 2 });
            var result = target.Add(null!, EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Single(x => x.Key.Value == "key1").Key.CachedPayload);
        }

        [Fact]
        public void Add_ExistingGroupHasDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, null));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(3, "key1"), new TestFact(4, null));
            var result = target.Add(null!, EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(2, aggregate1[GetKey("key1")].Count());
            Assert.Equal(2, aggregate1[GetKey(null)].Count());
        }

        [Fact]
        public void Add_NewAndExistingGroups_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var toAdd = AsFact(new TestFact(2, "key1"), new TestFact(3, "key2"), new TestFact(4, "key2"));
            var result = target.Add(null!, EmptyTuple(), toAdd).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Equal(2, aggregate1[GetKey("key1")].Count());
            Assert.Equal(2, aggregate1[GetKey("key2")].Count());
        }

        [Fact]
        public void Modify_ExistingGroupsSameIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key1");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_ExistingGroupsDifferentIdentity_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(3, "key1");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_KeyPayloadChanges_CachedPayloadUpdated()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1") { Payload = 1 }, new TestFact(2, "key1") { Payload = 1 });
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key1") { Payload = 2 };
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Single(x => x.Key.Value == "key1").Key.CachedPayload);
        }

        [Fact]
        public void Modify_ExistingGroupsHasDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, null);
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
        }

        [Fact]
        public void Modify_GroupRemovedAndAdded_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"), new TestFact(3, "key2"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key2");
            facts[1].Value = new TestFact(2, "key1");
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Single(aggregate1[GetKey("key1")]);
            Assert.Equal(2, aggregate1[GetKey("key2")].Count());
        }

        [Fact]
        public void Modify_ExistingGroupKeyChanged_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, "key2");
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Single(aggregate1[GetKey("key1")]);
            Assert.Single(aggregate1[GetKey("key2")]);
        }

        [Fact]
        public void Modify_ExistingGroupKeyChangedToDefault_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(2, null);
            var toUpdate = facts.Take(1).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Equal(2, aggregate1.Count());
            Assert.Single(aggregate1[GetKey("key1")]);
            Assert.Single(aggregate1[GetKey(null)]);
        }

        [Fact]
        public void Modify_ExistingGroupAllElementsHaveKeyChanged_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key2");
            facts[1].Value = new TestFact(2, "key2");
            var toUpdate = facts.Take(2).ToArray();
            var result = target.Modify(null!, EmptyTuple(), toUpdate).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Single(aggregate1);
            Assert.Equal(2, aggregate1[GetKey("key2")].Count());
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(null!, EmptyTuple(), AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"))));
        }

        [Fact]
        public void Modify_NonExistentDefaultKey_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(null!, EmptyTuple(), AsFact(new TestFact(1, null))));
        }

        [Fact]
        public void Remove_ExistingGroup_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, "key1");
            var toRemove = facts.Take(1).ToArray();
            var result = target.Remove(null!, EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Single(aggregate1);
            Assert.Single(aggregate1[GetKey("key1")]);
        }

        [Fact]
        public void Remove_ExistingGroupWithDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            facts[0].Value = new TestFact(1, null);
            var toRemove = facts.Take(1).ToArray();
            var result = target.Remove(null!, EmptyTuple(), toRemove).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey?, GroupElement>)result[0].Aggregate;
            Assert.Single(aggregate1);
            Assert.Single(aggregate1[GetKey(null)]);
        }

        [Fact]
        public void Remove_ExistingGroupAllElementsRemoved_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var result = target.Remove(null!, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey, GroupElement>)result[0].Aggregate;
            Assert.Empty(aggregate1);
        }

        [Fact]
        public void Remove_ExistingGroupAllElementsRemovedDefaultKey_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
            target.Add(null!, EmptyTuple(), facts);

            //Act
            var result = target.Remove(null!, EmptyTuple(), facts).ToArray();

            //Assert
            Assert.Single(result);
            Assert.Equal(AggregationAction.Modified, result[0].Action);
            var aggregate1 = (ILookup<GroupKey, GroupElement>)result[0].Aggregate;
            Assert.Empty(aggregate1);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(null!, EmptyTuple(), AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"))));
        }

        [Fact]
        public void Remove_NonExistentDefaultKey_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(null!, EmptyTuple(), AsFact(new TestFact(1, null))));
        }

        private LookupAggregator<TestFact, GroupKey, GroupElement> CreateTarget()
        {
            var keyExpression = new FactExpression<TestFact, GroupKey?>(GetGroupKey);
            var elementExpression = new FactExpression<TestFact, GroupElement>(x => new GroupElement(x));
            return new LookupAggregator<TestFact, GroupKey, GroupElement>(keyExpression, elementExpression);
        }

        private static GroupKey? GetGroupKey(TestFact fact)
        {
            return fact.Key == null ? null : new GroupKey(fact.Key, fact.Payload);
        }

        private static GroupKey? GetKey(string? key)
        {
            return key == null ? null : new GroupKey(key, 0);
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id, string? key)
            {
                Id = id;
                Key = key;
            }

            public int Id { get; }
            public string? Key { get; }
            public int Payload { get; set; } = 0;

            public bool Equals(TestFact? other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Id == other.Id;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;
                return Equals((TestFact)obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }

        private class GroupKey : IEquatable<GroupKey>
        {
            public GroupKey(string? value, int payload)
            {
                Value = value;
                CachedPayload = payload;
            }

            public string? Value { get; }
            public int CachedPayload { get; }

            public bool Equals(GroupKey? other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return string.Equals(Value, other.Value);
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;
                return Equals((GroupKey)obj);
            }

            public override int GetHashCode()
            {
                return (Value != null ? Value.GetHashCode() : 0);
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
            public string? Key { get; }
        }
    }
}