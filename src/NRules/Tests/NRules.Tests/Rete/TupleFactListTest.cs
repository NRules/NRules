using NRules.Rete;
using Xunit;

namespace NRules.Tests.Rete;

public class TupleFactListTest
{
    [Fact]
    public void Count_Empty_Zero()
    {
        // Arrange
        var list = new TupleFactList();

        // Act
        var count = list.Count;

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void Count_NonEmpty_CorrectValue()
    {
        // Arrange
        var list = new TupleFactList();
        list.Add(new Tuple(1), new Fact("value1"));
        list.Add(new Tuple(2), new Fact("value2"));

        // Act
        var count = list.Count;

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void Enumerate_Empty_NoValues()
    {
        // Arrange
        var list = new TupleFactList();

        // Act
        var enumerator = list.GetEnumerator();
        
        // Assert
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void Enumerate_UniqueTupleValues_ReturnsValues()
    {
        // Arrange
        var list = new TupleFactList();
        list.Add(new Tuple(1), new Fact("value1"));
        list.Add(new Tuple(2), new Fact("value2"));

        // Act - Assert
        var enumerator = list.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(1, enumerator.CurrentTuple.Id);
        Assert.NotNull(enumerator.CurrentFact);
        Assert.Equal("value1", enumerator.CurrentFact.Object);

        Assert.True(enumerator.MoveNext());
        Assert.Equal(2, enumerator.CurrentTuple.Id);
        Assert.NotNull(enumerator.CurrentFact);
        Assert.Equal("value2", enumerator.CurrentFact.Object);
        
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void Enumerate_RepeatedTupleValues_ReturnsValues()
    {
        // Arrange
        var list = new TupleFactList();
        var tuple1 = new Tuple(1);
        list.Add(tuple1, new Fact("value1"));
        list.Add(tuple1, new Fact("value2"));
        var tuple2 = new Tuple(2);
        list.Add(tuple2, new Fact("value3"));

        // Act - Assert
        var enumerator = list.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(1, enumerator.CurrentTuple.Id);
        Assert.NotNull(enumerator.CurrentFact);
        Assert.Equal("value1", enumerator.CurrentFact.Object);

        Assert.True(enumerator.MoveNext());
        Assert.Equal(1, enumerator.CurrentTuple.Id);
        Assert.NotNull(enumerator.CurrentFact);
        Assert.Equal("value2", enumerator.CurrentFact.Object);
        
        Assert.True(enumerator.MoveNext());
        Assert.Equal(2, enumerator.CurrentTuple.Id);
        Assert.NotNull(enumerator.CurrentFact);
        Assert.Equal("value3", enumerator.CurrentFact.Object);
        
        Assert.False(enumerator.MoveNext());
    }
}
