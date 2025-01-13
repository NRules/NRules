using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NRules.Aggregators;
using NRules.Diagnostics;
using Xunit;

namespace NRules.Tests.Aggregators;

public class GroupByAggregatorTest : AggregatorTest
{
    [Fact]
    public void Add_NewGroupNewInstance_AddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"), new TestFact(3, "key2"));
        var result = target.Add(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Added, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Equal(2, aggregate1.Count());
        Assert.Equal(AggregationAction.Added, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Equal("key2", aggregate2.Key.Value);
        Assert.Single(aggregate2);
    }

    [Fact]
    public void Add_NewGroupHasDefaultKey_AddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, null));
        var result = target.Add(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Added, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Single(aggregate1);
        Assert.Equal(AggregationAction.Added, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Null(aggregate2.Key);
        Assert.Single(aggregate2);
    }

    [Fact]
    public void Add_NewGroupExistingInstance_AddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toAdd = AsFact(new TestFact(3, "key2"), new TestFact(4, "key2"));
        var result = target.Add(context, EmptyTuple(), toAdd).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Added, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key2", aggregate1.Key.Value);
        Assert.Equal(2, aggregate1.Count());
    }

    [Fact]
    public void Add_ExistingGroup_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toAdd = AsFact(new TestFact(3, "key1"), new TestFact(4, "key2"));
        var result = target.Add(context, EmptyTuple(), toAdd).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Equal(2, aggregate1.Count());
        Assert.Equal(AggregationAction.Modified, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Equal("key2", aggregate2.Key.Value);
        Assert.Equal(2, aggregate2.Count());
    }

    [Fact]
    public void Add_KeyPayloadChanges_KeyPayloadUpdated()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1") {Payload = 1}, new TestFact(2, "key2") {Payload = 1});
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toAdd = AsFact(new TestFact(3, "key1") {Payload = 2});
        var result = target.Add(context, EmptyTuple(), toAdd).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>)result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Equal(2, aggregate1.Key.CachedPayload);
    }

    [Fact]
    public void Add_ExistingGroupHasDefaultKey_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, null));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toAdd = AsFact(new TestFact(3, "key1"), new TestFact(4, null));
        var result = target.Add(context, EmptyTuple(), toAdd).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Equal(2, aggregate1.Count());
        Assert.Equal(AggregationAction.Modified, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Null(aggregate2.Key);
        Assert.Equal(2, aggregate2.Count());
    }

    [Fact]
    public void Add_NewAndExistingGroups_AddedAndModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toAdd = AsFact(new TestFact(2, "key1"), new TestFact(3, "key2"), new TestFact(4, "key2"));
        var result = target.Add(context, EmptyTuple(), toAdd).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Equal(2, aggregate1.Count());
        Assert.Equal(AggregationAction.Added, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Equal("key2", aggregate2.Key.Value);
        Assert.Equal(2, aggregate2.Count());
    }

    [Fact]
    public void Add_EmptyGroup_NoResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act
        var result = target.Add(context, EmptyTuple(), AsFact(Array.Empty<TestFact>())).ToArray();

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Modify_ExistingGroupsSameIdentity_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, "key1");
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
    }

    [Fact]
    public void Modify_ExistingGroupsDifferentIdentity_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(3, "key1");
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
    }

    [Fact]
    public void Modify_KeyPayloadChanges_CachedPayloadUpdated()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1") {Payload = 1}, new TestFact(2, "key1") {Payload = 1});
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, "key1") {Payload = 2};
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>)result[0].Aggregate;
        Assert.Equal(2, aggregate1.Key.CachedPayload);
    }

    [Fact]
    public void Modify_ExistingGroupsHasDefaultKey_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, null);
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
    }

    [Fact]
    public void Modify_GroupRemovedAndAdded_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"), new TestFact(3, "key2"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, "key2");
        facts[1].Value = new TestFact(2, "key1");
        var toUpdate = facts.Take(2).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        Assert.Equal(AggregationAction.Modified, result[1].Action);
        Assert.Same(result[0].Previous, result[0].Aggregate);
        Assert.Same(result[1].Previous, result[1].Aggregate);
        Assert.Single(((IEnumerable<GroupElement>) result[0].Aggregate));
        Assert.Equal(2, ((IEnumerable<GroupElement>) result[1].Aggregate).Count());
    }

    [Fact]
    public void Modify_ExistingGroupKeyChanged_ModifiedAndAddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(2, "key2");
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Single(aggregate1);
        Assert.Equal(AggregationAction.Added, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Equal("key2", aggregate2.Key.Value);
        Assert.Single(aggregate2);
    }

    [Fact]
    public void Modify_ExistingGroupKeyChangedToDefault_ModifiedAndAddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(2, null);
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Single(aggregate1);
        Assert.Equal(AggregationAction.Added, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Null(aggregate2.Key);
        Assert.Single(aggregate2);
    }

    [Fact]
    public void Modify_ExistingGroupAllElementsHaveKeyChanged_RemovedAndAddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, "key2");
        facts[1].Value = new TestFact(2, "key2");
        var toUpdate = facts.Take(2).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Empty(aggregate1);
        Assert.Equal(AggregationAction.Added, result[1].Action);
        var aggregate2 = (IGrouping<GroupKey, GroupElement>) result[1].Aggregate;
        Assert.Equal("key2", aggregate2.Key.Value);
        Assert.Equal(2, aggregate2.Count());
    }

    [Fact]
    public void Modify_NonExistent_Throws()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act - Assert
        Assert.Throws<KeyNotFoundException>(
            () => target.Modify(context, EmptyTuple(), AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"))));
    }

    [Fact]
    public void Modify_NonExistentDefaultKey_Throws()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act - Assert
        Assert.Throws<KeyNotFoundException>(
            () => target.Modify(context, EmptyTuple(), AsFact(new TestFact(1, null))));
    }

    [Fact]
    public void Remove_ExistingGroup_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, "key1");
        var toRemove = facts.Take(1).ToArray();
        var result = target.Remove(context, EmptyTuple(), toRemove).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Single(aggregate1);
    }

    [Fact]
    public void Remove_ExistingGroupWithDefaultKey_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact(1, null);
        var toRemove = facts.Take(1).ToArray();
        var result = target.Remove(context, EmptyTuple(), toRemove).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Null(aggregate1.Key);
        Assert.Single(aggregate1);
    }

    [Fact]
    public void Remove_ExistingGroupAllElementsRemoved_RemovedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, "key1"), new TestFact(2, "key1"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var result = target.Remove(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Equal("key1", aggregate1.Key.Value);
        Assert.Empty(aggregate1);
    }

    [Fact]
    public void Remove_ExistingGroupAllElementsRemovedDefaultKey_RemovedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact(1, null), new TestFact(2, null));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var result = target.Remove(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Single(result);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        var aggregate1 = (IGrouping<GroupKey, GroupElement>) result[0].Aggregate;
        Assert.Null(aggregate1.Key);
        Assert.Empty(aggregate1);
    }

    [Fact]
    public void Remove_NonExistent_Throws()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act - Assert
        Assert.Throws<KeyNotFoundException>(
            () => target.Remove(context, EmptyTuple(), AsFact(new TestFact(1, "key1"), new TestFact(2, "key2"))));
    }

    [Fact]
    public void Remove_NonExistentDefaultKey_Throws()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act - Assert
        Assert.Throws<KeyNotFoundException>(
            () => target.Remove(context, EmptyTuple(), AsFact(new TestFact(1, null))));
    }
    
    private static AggregationContext GetContext()
    {
        var mockExecutionContext = new Mock<IExecutionContext>();
        return new AggregationContext(mockExecutionContext.Object, new NodeInfo());
    }

    private GroupByAggregator<TestFact, GroupKey, GroupElement> CreateTarget()
    {
        var keyExpression = new FactExpression<TestFact, GroupKey?>(GetGroupKey);
        var elementExpression = new FactExpression<TestFact, GroupElement>(x => new GroupElement(x));
        return new GroupByAggregator<TestFact, GroupKey, GroupElement>(keyExpression, elementExpression);
    }

    private static GroupKey? GetGroupKey(TestFact fact)
    {
        return fact.Key == null ? null : new GroupKey(fact.Key, fact.Payload);
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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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