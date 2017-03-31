using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.RuleModel.Aggregators;
using NUnit.Framework;

namespace NRules.Tests.Aggregators
{
    [TestFixture]
    public class ProjectionAggregatorTest
    {
        [Test]
        public void Aggregates_NewInstance_Empty()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Aggregates_HasElements_AllElements()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "value1"), new TestFact(2, "value2")});

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void Add_Facts_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new[] { new TestFact(1, "value1"), new TestFact(2, "value2") }).ToArray();

            //Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(AggregationAction.Added, result[0].Action);
            Assert.AreEqual("value1", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[1].Action);
            Assert.AreEqual("value2", result[1].Aggregate);
        }

        [Test]
        public void Add_NoFacts_EmptyResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new TestFact[0]).ToArray();

            //Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Modify_Existing_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3")});

            //Act
            var result = target.Modify(new[] {new TestFact(1, "value1"), new TestFact(2, "value2")}).ToArray();

            //Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(AggregationAction.Modified, result[0].Action);
            Assert.AreEqual("value1", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Modified, result[1].Action);
            Assert.AreEqual("value2", result[1].Aggregate);
        }

        [Test]
        public void Modify_ProjectionChanged_RemovedAddedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3")});

            //Act
            var result = target.Modify(new[] {new TestFact(1, "value1x"), new TestFact(2, "value2x")}).ToArray();

            //Assert
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(AggregationAction.Removed, result[0].Action);
            Assert.AreEqual("value1", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[1].Action);
            Assert.AreEqual("value1x", result[1].Aggregate);
            Assert.AreEqual(AggregationAction.Removed, result[2].Action);
            Assert.AreEqual("value2", result[2].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[3].Action);
            Assert.AreEqual("value2x", result[3].Aggregate);
        }

        [Test]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(new[] {new TestFact(1, "value1"), new TestFact(2, "value2")}));
        }

        [Test]
        public void Remove_Existing_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "value1"), new TestFact(2, "value2"), new TestFact(3, "value3")});

            //Act
            var result = target.Remove(new[] {new TestFact(1, "value1"), new TestFact(2, "value2")}).ToArray();

            //Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(AggregationAction.Removed, result[0].Action);
            Assert.AreEqual("value1", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Removed, result[1].Action);
            Assert.AreEqual("value2", result[1].Aggregate);
        }

        [Test]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(new[] {new TestFact(1, "value1"), new TestFact(2, "value2")}));
        }

        private ProjectionAggregator<TestFact, string> CreateTarget()
        {
            return new ProjectionAggregator<TestFact, string>(x => x.Value);
        }

        private class TestFact : IEquatable<TestFact>
        {
            private readonly int _id;
            private readonly string _value;

            public TestFact(int id, string value)
            {
                _id = id;
                _value = value;
            }

            public int Id { get { return _id; } }
            public string Value { get { return _value; } }

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