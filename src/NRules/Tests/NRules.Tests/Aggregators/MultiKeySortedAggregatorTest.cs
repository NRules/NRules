using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using NRules.RuleModel;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class MultiKeySortedAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_NewInstance_AddedResult_AscendingThenAscending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndInt2(SortDirection.Ascending, SortDirection.Ascending);
            var fact1_2 = new TestFact(1, 2);
            var fact1_1 = new TestFact(1, 1);
            var fact2_3 = new TestFact(2, 3);

            // Act
            var facts = AsFact(fact2_3, fact1_2, fact1_1);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, fact1_1, fact1_2, fact2_3);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_AscendingThenDescending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndInt2(SortDirection.Ascending, SortDirection.Descending);
            var fact1_2 = new TestFact(1, 2);
            var fact1_1 = new TestFact(1, 1);
            var fact2_3 = new TestFact(2, 3);

            // Act
            
            var facts = AsFact(fact2_3, fact1_2, fact1_1);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, fact1_2, fact1_1, fact2_3);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_DescendingThenAscending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndInt2(SortDirection.Descending, SortDirection.Ascending);
            var fact1_2 = new TestFact(1, 2);
            var fact1_1 = new TestFact(1, 1);
            var fact2_3 = new TestFact(2, 3);

            // Act
            var facts = AsFact(fact2_3, fact1_2, fact1_1);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, fact2_3, fact1_1, fact1_2);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_DescendingThenDescending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndInt2(SortDirection.Descending, SortDirection.Descending);
            var fact1_2 = new TestFact(1, 2);
            var fact1_1 = new TestFact(1, 1);
            var fact2_3 = new TestFact(2, 3);

            // Act
            var facts = AsFact(fact2_3, fact1_2, fact1_1);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, fact2_3, fact1_2, fact1_1);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_IntThenString_DescendingThenAscending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Descending, SortDirection.Ascending);
            var fact1_B = new TestFact(1, "B");
            var fact1_A = new TestFact(1, "A");
            var fact2_C = new TestFact(2, "C");

            // Act
            var facts = AsFact(fact2_C, fact1_B, fact1_A);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, fact2_C, fact1_A, fact1_B);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_IntThenString_DescendingThenDescending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Descending, SortDirection.Descending);
            var fact1_B = new TestFact(1, "B");
            var fact1_A = new TestFact(1, "A");
            var fact2_C = new TestFact(2, "C");

            // Act
            var facts = AsFact(fact2_C, fact1_B, fact1_A);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, fact2_C, fact1_B, fact1_A);
        }

        [Fact]
        public void Add_NoFacts_AddedResultEmptyCollection()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndInt2(SortDirection.Descending, SortDirection.Descending);

            // Act
            var facts = AsFact(new TestFact[0]);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added);
        }

        [Fact]
        public void Add_NewInstanceDuplicateFacts_AddedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Descending, SortDirection.Ascending);
            var duplicateFact1 = new TestFact(1, "A");
            var duplicateFact2 = new TestFact(1, "A");

            // Act
            var facts = AsFact(duplicateFact1, duplicateFact2);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, duplicateFact1, duplicateFact2);
        }

        [Fact]
        public void Add_OldInstanceNewFacts_ModifiedResult_AscendingThenDescending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Descending);
            var fact1 = new TestFact(1, "B");
            var fact3 = new TestFact(2, "C");
            target.Add(null, EmptyTuple(), AsFact(fact1, fact3));

            // Act
            var fact2 = new TestFact(1, "A");
            var facts = AsFact(fact2);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1, fact2, fact3);
        }

        [Fact]
        public void Add_OldInstanceNewFacts_ModifiedResult_DescendingThenAscending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Descending);
            var fact1 = new TestFact(1, "B");
            var fact2 = new TestFact(1, "A");
            var fact3 = new TestFact(2, "C");
            target.Add(null, EmptyTuple(), AsFact(fact1, fact3));

            // Act
            var facts = AsFact(fact2);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1, fact2, fact3);
        }

        [Fact]
        public void Add_OldInstanceDuplicateFacts_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var duplicateFact1 = new TestFact(1, "A");
            var duplicateFact2 = new TestFact(1, "A");

            // Act
            var facts1 = AsFact(duplicateFact1);
            target.Add(null, EmptyTuple(), facts1).ToArray();

            var facts2 = AsFact(duplicateFact2);
            var result = target.Add(null, EmptyTuple(), facts2).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, duplicateFact1, duplicateFact2);
        }

        [Fact]
        public void Modify_ExistingFactsSameValues_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(2, "B");

            var facts = AsFact(fact1, fact2);
            target.Add(null, EmptyTuple(), facts);

            // Act
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1, fact2);
        }

        [Fact]
        public void Modify_ExistingFactsDifferentValues_ModifiedResult_AscendingThenAscending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "B");
            var facts = AsFact(fact1, fact2);
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(1, "D");
            facts[1].Value = new TestFact(1, "C");
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, new TestFact(1, "C"), new TestFact(1, "D"));
        }

        [Fact]
        public void Modify_ExistingFactsDifferentValues_ModifiedResult_AscendingThenDescending()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Descending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "B");

            var facts = AsFact(fact1, fact1);
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(1, "D");
            facts[1].Value = new TestFact(1, "C");
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, new TestFact(1, "D"), new TestFact(1, "C"));
        }

        [Fact]
        public void Modify_ExistingDuplicateFacts_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "A");
            var facts = AsFact(fact1, fact2);
            target.Add(null, EmptyTuple(), facts);

            // Act
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1, fact2);
        }

        [Fact]
        public void Modify_ExistingDuplicateFactsDeduplicated_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "A");
            var facts = AsFact(fact1, fact2);
            target.Add(null, EmptyTuple(), facts);

            // Act
            fact1 = new TestFact(1, "B");
            facts[0].Value = fact1;
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact2, fact1);
        }

        [Fact]
        public void Modify_ExistingDuplicateFactsDeduplicatedOneByOne_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var facts = AsFact(new TestFact(1, "A"), new TestFact(1, "A"));
            target.Add(null, EmptyTuple(), facts);

            // Act
            var fact1 = new TestFact(1, "B");
            facts[0].Value = fact1;
            target.Modify(null, EmptyTuple(), facts.Take(1)).ToArray();

            var fact2 = new TestFact(1, "C");
            facts[1].Value = fact2;
            var result = target.Modify(null, EmptyTuple(), facts.Skip(1).Take(1)).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1, fact2);
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var facts = AsFact(new TestFact(1, "A"), new TestFact(1, "B"));
            target.Add(null, EmptyTuple(), facts);

            // Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(null, EmptyTuple(), AsFact(new TestFact(1, "A"), new TestFact(1, "B"))));
        }

        [Fact]
        public void Remove_ExistingFactsSameValues_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "B");
            var fact3 = new TestFact(1, "C");
            var facts = AsFact(fact1, fact2, fact3);
            target.Add(null, EmptyTuple(), facts);

            // Act
            var toRemove = new[] { facts.ElementAt(1) };
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1, fact3);
        }

        [Fact]
        public void Remove_ExistingFactsDifferentValues_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "B");
            var fact3 = new TestFact(1, "C");
            var facts = AsFact(fact1, fact2, fact3);
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(2, "A");
            facts[1].Value = new TestFact(2, "B");
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact3);
        }

        [Fact]
        public void Remove_ExistingDuplicateFacts_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "B");
            var fact3 = new TestFact(1, "B");
            var facts = AsFact(fact1, fact2, fact3);
            target.Add(null, EmptyTuple(), facts);

            // Act
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact3);
        }

        [Fact]
        public void Remove_ExistingDuplicateFactsRemovedOneByOne_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var fact1 = new TestFact(1, "A");
            var fact2 = new TestFact(1, "B");
            var fact3 = new TestFact(1, "B");
            var facts = AsFact(fact1, fact2, fact3);
            target.Add(null, EmptyTuple(), facts);

            // Act
            target.Remove(null, EmptyTuple(), facts.Skip(1).Take(1).ToArray()).ToArray();
            var result = target.Remove(null, EmptyTuple(), facts.Skip(2).Take(1).ToArray()).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, fact1);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            // Arrange
            var target = CreateTarget_SortWithInt1AndString(SortDirection.Ascending, SortDirection.Ascending);
            var facts = AsFact(new TestFact(1, "A"), new TestFact(1, "B"));
            target.Add(null, EmptyTuple(), facts);

            // Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(null, EmptyTuple(), AsFact(new TestFact(1, "A"), new TestFact(1, "B"))));
        }

        private static MultiKeySortedAggregator<TestFact> CreateTarget_SortWithInt1AndInt2(SortDirection sortDirection1, SortDirection sortDirection2)
        {
            var expression1 = new FactExpression<TestFact, int>(x => x.Int1);
            var expression2 = new FactExpression<TestFact, int>(x => x.Int2);
            var sortCondition1 = new SortCondition(AggregateElement.KeySelectorAscendingName, sortDirection1, expression1);
            var sortCondition2 = new SortCondition(AggregateElement.KeySelectorAscendingName, sortDirection2, expression2);
            return new MultiKeySortedAggregator<TestFact>(new[] { sortCondition1, sortCondition2 });
        }

        private static MultiKeySortedAggregator<TestFact> CreateTarget_SortWithInt1AndString(SortDirection sortDirectionInt, SortDirection sortDirectionString)
        {
            var expressionInt = new FactExpression<TestFact, int>(x => x.Int1);
            var expressionString = new FactExpression<TestFact, string>(x => x.String);
            var sortCondition1 = new SortCondition(AggregateElement.KeySelectorAscendingName, sortDirectionInt, expressionInt);
            var sortCondition2 = new SortCondition(AggregateElement.KeySelectorAscendingName, sortDirectionString, expressionString);
            return new MultiKeySortedAggregator<TestFact>(new[] { sortCondition1, sortCondition2 });
        }

        private static void AssertAggregationResult(AggregationResult[] results, AggregationAction action, params TestFact[] orderedFacts)
        {
            Assert.Equal(1, results.Length);

            var result = results[0];
            Assert.Equal(action, result.Action);

            var aggregate = (IEnumerable<TestFact>)result.Aggregate;
            Assert.Equal(action == AggregationAction.Added ? null : aggregate, result.Previous);

            var actualAggregateFacts = aggregate.ToArray();
            var actualSourceFacts = result.Source.Select(f => (TestFact)f.Value).ToArray();
            Assert.Equal(orderedFacts.Length, actualSourceFacts.Length);
            for (var i = 0; i < orderedFacts.Length; i++)
            {
                Assert.Equal(orderedFacts[i], actualAggregateFacts[i]);
                Assert.Equal(orderedFacts[i], actualSourceFacts[i]);
            }
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int value1, int value2)
            {
                Int1 = value1;
                Int2 = value2;
            }

            public TestFact(int value1, string stringValue)
            {
                Int1 = value1;
                String = stringValue;
            }

            public int Int1 { get; }
            public int Int2 { get; }
            public string String { get; }

            public bool Equals(TestFact other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Int1 == other.Int1 && Int2 == other.Int2 && String == other.String;
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
                return Int1.GetHashCode() ^ Int2.GetHashCode() ^ String.GetHashCode();
            }
        }
    }
}