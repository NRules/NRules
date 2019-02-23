using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using NRules.RuleModel;
using Xunit;

namespace NRules.Tests.Aggregators
{
    public class SortedAggregatorTest : AggregatorTest
    {
        [Fact]
        public void Add_NewInstance_AddedResult_Ascending()
        {
            // Arrange
            var target = CreateTarget();

            // Act
            var facts = AsFact(new TestFact(3), new TestFact(1), new TestFact(2));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, 1, 2, 3);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_Descending()
        {
            // Arrange
            var target = CreateTarget(SortDirection.Descending);

            // Act
            var facts = AsFact(new TestFact(2), new TestFact(1), new TestFact(3));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, 3, 2, 1);
        }

        [Fact]
        public void Add_NewInstance_AddedResult_String_Ascending()
        {
            // Arrange
            var target = CreateTarget_SortByValue();

            // Act
            var facts = AsFact(new TestFact(3, "A"), new TestFact(1, "C"), new TestFact(2, "B"));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, "A", "B", "C");
        }

        [Fact]
        public void Add_NewInstance_AddedResult_String_Descending()
        {
            // Arrange
            var target = CreateTarget_SortByValue(SortDirection.Descending);

            // Act
            var facts = AsFact(new TestFact(2, "C"), new TestFact(1, "B"), new TestFact(3, "A"));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, "C", "B", "A");
        }

        [Fact]
        public void Add_NoFacts_AddedResultEmptyCollection()
        {
            // Arrange
            var target = CreateTarget();

            // Act
            var facts = AsFact(new TestFact[0]);
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, new int[0]);
        }

        [Fact]
        public void Add_NewInstanceDuplicateFacts_AddedResult()
        {
            // Arrange
            var target = CreateTarget();

            // Act
            var facts = AsFact(new TestFact(1), new TestFact(1));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Added, 1, 1);
        }

        [Fact]
        public void Add_OldInstanceNewFacts_ModifiedResult_Ascending()
        {
            // Arrange
            var target = CreateTarget();
            target.Add(null, EmptyTuple(), AsFact(new TestFact(5)));

            // Act
            var facts = AsFact(new TestFact(6), new TestFact(4));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 4, 5, 6);
        }

        [Fact]
        public void Add_OldInstanceNewFacts_ModifiedResult_Descending()
        {
            // Arrange
            var target = CreateTarget(SortDirection.Descending);
            target.Add(null, EmptyTuple(), AsFact(new TestFact(5)));

            // Act
            var facts = AsFact(new TestFact(4), new TestFact(6));
            var result = target.Add(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 6, 5, 4);
        }

        [Fact]
        public void Add_OldInstanceDuplicateFacts_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();

            // Act
            var facts1 = AsFact(new TestFact(1));
            target.Add(null, EmptyTuple(), facts1).ToArray();

            var facts2 = AsFact(new TestFact(1));
            var result = target.Add(null, EmptyTuple(), facts2).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 1, 1);
        }

        [Fact]
        public void Modify_ExistingFactsSameValues_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(null, EmptyTuple(), facts);

            // Act
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 1, 2);
        }

        [Fact]
        public void Modify_ExistingFactsDifferentValues_ModifiedResult_Ascending()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(4);
            facts[1].Value = new TestFact(3);
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 3, 4);
        }

        [Fact]
        public void Modify_ExistingFactsDifferentValues_ModifiedResult_Descending()
        {
            // Arrange
            var target = CreateTarget(SortDirection.Descending);
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(3);
            facts[1].Value = new TestFact(4);
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 4, 3);
        }

        [Fact]
        public void Modify_ExistingDuplicateFacts_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(1));
            target.Add(null, EmptyTuple(), facts);

            // Act
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 1, 1);
        }

        [Fact]
        public void Modify_ExistingDuplicateFactsDeduplicated_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(1));
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(2);
            var result = target.Modify(null, EmptyTuple(), facts).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 1, 2);
        }

        [Fact]
        public void Modify_ExistingDuplicateFactsDeduplicatedOneByOne_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(1));
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(3);
            target.Modify(null, EmptyTuple(), facts.Take(1)).ToArray();

            facts[1].Value = new TestFact(2);
            var result = target.Modify(null, EmptyTuple(), facts.Skip(1).Take(1)).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 2, 3);
        }

        [Fact]
        public void Modify_NonExistent_Throws()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2));
            target.Add(null, EmptyTuple(), facts);

            // Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(null, EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))));
        }

        [Fact]
        public void Remove_ExistingFactsSameValues_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(3));
            target.Add(null, EmptyTuple(), facts);

            // Act
            var toRemove = new[] { facts.ElementAt(1) };
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 1, 3);
        }

        [Fact]
        public void Remove_ExistingFactsDifferentValues_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(3));
            target.Add(null, EmptyTuple(), facts);

            // Act
            facts[0].Value = new TestFact(3);
            facts[1].Value = new TestFact(4);
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 3);
        }

        [Fact]
        public void Remove_ExistingDuplicateFacts_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(2));
            target.Add(null, EmptyTuple(), facts);

            // Act
            var toRemove = facts.Take(2).ToArray();
            var result = target.Remove(null, EmptyTuple(), toRemove).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 2);
        }

        [Fact]
        public void Remove_ExistingDuplicateFactsRemovedOneByOne_ModifiedResult()
        {
            // Arrange
            var target = CreateTarget();
            var facts = AsFact(new TestFact(1), new TestFact(2), new TestFact(2));
            target.Add(null, EmptyTuple(), facts);

            // Act
            target.Remove(null, EmptyTuple(), facts.Skip(1).Take(1).ToArray()).ToArray();
            var result = target.Remove(null, EmptyTuple(), facts.Skip(2).Take(1).ToArray()).ToArray();

            // Assert
            AssertAggregationResult(result, AggregationAction.Modified, 1);
        }

        [Fact]
        public void Remove_NonExistent_Throws()
        {
            // Arrange
            var target = CreateTarget();
            target.Add(null, EmptyTuple(), AsFact(new TestFact(1), new TestFact(2)));

            // Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(null, EmptyTuple(), AsFact(new TestFact(1), new TestFact(2))));
        }

        private static SortedAggregator<TestFact, int> CreateTarget(SortDirection sortDirection = SortDirection.Ascending)
        {
            var expression = new FactExpression<TestFact, int>(x => x.Id);
            return new SortedAggregator<TestFact, int>(expression, sortDirection);
        }

        private static SortedAggregator<TestFact, string> CreateTarget_SortByValue(SortDirection sortDirection = SortDirection.Ascending)
        {
            var expression = new FactExpression<TestFact, string>(x => x.Value);
            return new SortedAggregator<TestFact, string>(expression, sortDirection);
        }

        private static void AssertAggregationResult(AggregationResult[] results, AggregationAction action, params int[] orderedKeys)
        {
            AssertAggregationResult(results, action, f => f.Id, orderedKeys);
        }

        private static void AssertAggregationResult(AggregationResult[] results, AggregationAction action, params string[] orderedKeys)
        {
            AssertAggregationResult(results, action, f => f.Value, orderedKeys);
        }

        private static void AssertAggregationResult<TKey>(AggregationResult[] results, AggregationAction action, Func<TestFact, TKey> keySelector, params TKey[] orderedKeys)
        {
            Assert.Equal(1, results.Length);

            var result = results[0];
            var distinctKeys = orderedKeys.Distinct().ToArray();
            Assert.Equal(action, result.Action);

            var aggregate = (IEnumerable<TestFact>)result.Aggregate;
            Assert.Equal(action == AggregationAction.Added ? null : aggregate, result.Previous);

            var actualAggregateKeys = aggregate.Select(keySelector).ToArray();
            var actualSourceKeys = result.Source.Select(f => keySelector((TestFact)f.Value)).ToArray();
            Assert.Equal(orderedKeys.Length, actualSourceKeys.Length);
            for (var i = 0; i < orderedKeys.Length; i++)
            {
                Assert.Equal(orderedKeys[i], actualAggregateKeys[i]);
                Assert.Equal(orderedKeys[i], actualSourceKeys[i]);
            }
        }

        private class TestFact : IEquatable<TestFact>
        {
            public TestFact(int id)
                : this(id, id.ToString())
            {
            }

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
                return Equals((TestFact)obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }
    }
}