using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.RuleModel.Aggregators;
using NUnit.Framework;

namespace NRules.Tests.Aggregators
{
    [TestFixture]
    public class FlatteningAggregatorTest
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
        public void Aggregates_HasElements_AllNestedElements()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] { new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22") });

            //Act
            var result = target.Aggregates.ToArray();

            //Assert
            Assert.AreEqual(4, result.Length);
        }

        [Test]
        public void Add_Facts_AddedResult()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            var result = target.Add(new[] {new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22")}).ToArray();

            //Assert
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(AggregationAction.Added, result[0].Action);
            Assert.AreEqual("value11", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[1].Action);
            Assert.AreEqual("value12", result[1].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[2].Action);
            Assert.AreEqual("value21", result[2].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[3].Action);
            Assert.AreEqual("value22", result[3].Aggregate);
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
        public void Modify_ExistingFacts_ModifiedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22")});

            //Act
            var result = target.Modify(new[] {new TestFact(1, "value11", "value12")}).ToArray();

            //Assert
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(AggregationAction.Removed, result[0].Action);
            Assert.AreEqual("value11", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Removed, result[1].Action);
            Assert.AreEqual("value12", result[1].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[2].Action);
            Assert.AreEqual("value11", result[2].Aggregate);
            Assert.AreEqual(AggregationAction.Added, result[3].Action);
            Assert.AreEqual("value12", result[3].Aggregate);
        }

        [Test]
        public void Modify_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Modify(new[] {new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22")}));
        }

        [Test]
        public void Remove_ExistingFacts_RemovedResult()
        {
            //Arrange
            var target = CreateTarget();
            target.Add(new[] {new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22")});

            //Act
            var result = target.Remove(new[] {new TestFact(1, "value11", "value12")}).ToArray();

            //Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(AggregationAction.Removed, result[0].Action);
            Assert.AreEqual("value11", result[0].Aggregate);
            Assert.AreEqual(AggregationAction.Removed, result[1].Action);
            Assert.AreEqual("value12", result[1].Aggregate);
        }

        [Test]
        public void Remove_NonExistent_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<KeyNotFoundException>(
                () => target.Remove(new[] {new TestFact(1, "value11", "value12"), new TestFact(2, "value21", "value22")}));
        }

        private FlatteningAggregator<TestFact, string> CreateTarget()
        {
            return new FlatteningAggregator<TestFact, string>(x => x.Values);
        }

        private class TestFact : IEquatable<TestFact>
        {
            private readonly int _id;
            private readonly string[] _values;

            public TestFact(int id, params string[] values)
            {
                _id = id;
                _values = values;
            }

            public int Id { get { return _id; } }
            public string[] Values { get { return _values; } }

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