using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NRules.Aggregators;
using NRules.Diagnostics;
using NRules.RuleModel;
using Xunit;

namespace NRules.Tests.Aggregators;

public class FlatteningAggregatorTest : AggregatorTest
{
    [Fact]
    public void Add_Facts_AddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act
        var facts = AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"));
        var result = target.Add(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Equal(4, result.Length);
        Assert.Equal(AggregationAction.Added, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[2].Action);
        Assert.Equal("value21", ((TestFact2)result[2].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[3].Action);
        Assert.Equal("value22", ((TestFact2)result[3].Aggregate).Value);
    }

    [Fact]
    public void Add_FactsWithDuplicates_AddedDistinctResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act
        var facts = AsFact(new TestFact1(1, "value11", "value12", "value12", "valuex"), new TestFact1(2, "value21", "value21", "value22", "valuex"));
        var result = target.Add(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Equal(5, result.Length);
        Assert.Equal(AggregationAction.Added, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[2].Action);
        Assert.Equal("valuex", ((TestFact2)result[2].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[3].Action);
        Assert.Equal("value21", ((TestFact2)result[3].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[4].Action);
        Assert.Equal("value22", ((TestFact2)result[4].Aggregate).Value);
    }

    [Fact]
    public void Add_NoFacts_EmptyResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act
        var facts = AsFact(Array.Empty<TestFact1>());
        var result = target.Add(context, EmptyTuple(), facts).ToArray();

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Modify_ExistingFactsSameIdentity_ModifiedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toUpdate = new []{facts[0]};
        facts[0].Value = new TestFact1(1, "value11", "value12");
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Modified, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Modified, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
    }

    [Fact]
    public void Modify_ExistingFactsDifferentIdentity_RemovedAddedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact1(3, "value31", "value32");
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(4, result.Length);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Removed, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[2].Action);
        Assert.Equal("value31", ((TestFact2)result[2].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[3].Action);
        Assert.Equal("value32", ((TestFact2)result[3].Aggregate).Value);
    }

    [Fact]
    public void Modify_ExistingFactsHasAdditionsModificationsAndRemovals_CorrectResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact1(2, "value12", "value13");
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(3, result.Length);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Modified, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[2].Action);
        Assert.Equal("value13", ((TestFact2)result[2].Aggregate).Value);
    }

    [Fact]
    public void Modify_FactsWithDuplicates_CorrectResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        var facts = AsFact(new TestFact1(1, "value11", "value12", "value12", "valuex"), new TestFact1(2, "value21", "value21", "value22", "valuex"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        facts[0].Value = new TestFact1(2, "value12", "value13");
        var toUpdate = facts.Take(1).ToArray();
        var result = target.Modify(context, EmptyTuple(), toUpdate).ToArray();

        //Assert
        Assert.Equal(3, result.Length);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Modified, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
        Assert.Equal(AggregationAction.Added, result[2].Action);
        Assert.Equal("value13", ((TestFact2)result[2].Aggregate).Value);
    }

    [Fact]
    public void Modify_NonExistent_Throws()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act - Assert
        Assert.Throws<KeyNotFoundException>(
            () => target.Modify(context, EmptyTuple(), AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"))));
    }

    [Fact]
    public void Remove_ExistingFacts_RemovedResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();
        var facts = AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"));
        target.Add(context, EmptyTuple(), facts);

        //Act
        var toRemove = facts.Take(1).ToArray();
        var result = target.Remove(context, EmptyTuple(), toRemove).ToArray();

        //Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(AggregationAction.Removed, result[0].Action);
        Assert.Equal("value11", ((TestFact2)result[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Removed, result[1].Action);
        Assert.Equal("value12", ((TestFact2)result[1].Aggregate).Value);
    }

    [Fact]
    public void Remove_FactsWithDuplicates_CorrectResult()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        var facts = AsFact(new TestFact1(1, "value11", "value12", "value12", "valuex"), new TestFact1(2, "value21", "value21", "value22", "valuex"));
        target.Add(context, EmptyTuple(), facts);

        //Act - I
        var toRemove1 = facts.Take(1).ToArray();
        var result1 = target.Remove(context, EmptyTuple(), toRemove1).ToArray();

        //Assert - I
        Assert.Equal(2, result1.Length);
        Assert.Equal(AggregationAction.Removed, result1[0].Action);
        Assert.Equal("value11", ((TestFact2)result1[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Removed, result1[1].Action);
        Assert.Equal("value12", ((TestFact2)result1[1].Aggregate).Value);

        //Act - II
        var toRemove2 = facts.Skip(1).Take(1).ToArray();
        var result2 = target.Remove(context, EmptyTuple(), toRemove2).ToArray();

        //Assert - II
        Assert.Equal(3, result2.Length);
        Assert.Equal(AggregationAction.Removed, result2[0].Action);
        Assert.Equal("value21", ((TestFact2)result2[0].Aggregate).Value);
        Assert.Equal(AggregationAction.Removed, result2[1].Action);
        Assert.Equal("value22", ((TestFact2)result2[1].Aggregate).Value);
        Assert.Equal(AggregationAction.Removed, result2[2].Action);
        Assert.Equal("valuex", ((TestFact2)result2[2].Aggregate).Value);
    }

    [Fact]
    public void Remove_NonExistent_Throws()
    {
        //Arrange
        var context = GetContext();
        var target = CreateTarget();

        //Act - Assert
        Assert.Throws<KeyNotFoundException>(
            () => target.Remove(context, EmptyTuple(), AsFact(new TestFact1(1, "value11", "value12"), new TestFact1(2, "value21", "value22"))));
    }

    private static AggregationContext GetContext()
    {
        var mockExecutionContext = new Mock<IExecutionContext>();
        return new AggregationContext(mockExecutionContext.Object, new NodeInfo());
    }
    
    private FlatteningAggregator<TestFact1, TestFact2> CreateTarget()
    {
        var identityComparer = new FactIdentityComparer(
            new DefaultFactIdentityComparer(), Array.Empty<FactIdentityComparerRegistry.Entry>());
        var expression = new FactExpression<TestFact1, IEnumerable<TestFact2>>(x => x.Values);
        var aggregator = new FlatteningAggregator<TestFact1, TestFact2>(identityComparer, expression);
        return aggregator;
    }

    private class TestFact1 : IEquatable<TestFact1>
    {
        public TestFact1(int id, params string[] values)
        {
            Id = id;
            Values = values.Select(v => new TestFact2(v)).ToArray();
        }

        public int Id { get; }
        public TestFact2[] Values { get; }

        public bool Equals(TestFact1? other)
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
            return Equals((TestFact1) obj);
        }
        
        public override int GetHashCode()
        {
            return Id;
        }
    }

    private class TestFact2 : IIdentityProvider
    {
        public string Value { get; }

        public TestFact2(string value)
        {
            Value = value;
        }
        
        public object GetIdentity() => Value;
    }
}