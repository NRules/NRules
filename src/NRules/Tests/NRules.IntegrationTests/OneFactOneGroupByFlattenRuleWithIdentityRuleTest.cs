using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactOneGroupByFlattenRuleWithIdentityRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_UpdatesWithSameIdButDifferentCount_FiresWithNewCount2()
        {
            //Arrange
            var factsToInsert = new object[]
            {
                new FactType {Id = 1, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group"},
                new FactType {Id = 2, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
                new FactType {Id = 3, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group3"},
                new FactType {Id = 4, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group2"}
            };
            var factsToUpdate = new object[]
            {
                new FactType {Id = 1, TestCount = 2, GroupingProperty = "GP1", GroupingProperty2 = "Group"},
                new FactType {Id = 2, TestCount = 3, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
                new FactType {Id = 3, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
                new FactType {Id = 4, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group"}
            };
            Session.InsertAll(factsToInsert);
            Session.UpdateAll(factsToUpdate);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedFacts = GetFiredFact<IGrouping<string, FactType>>();
            Assert.AreEqual(4, firedFacts.Count());
            Assert.AreEqual(1, firedFacts.Count(x => x.TestCount == 3));
            Assert.AreEqual(1, firedFacts.Count(x => x.TestCount == 2));
            Assert.AreEqual(2, firedFacts.Count(x => x.TestCount == 1));
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType : IEquatable<FactType>
        {
            public long Id { get; set; }
            public int TestCount { get; set; }
            public string GroupingProperty { get; set; }
            public string GroupingProperty2 { get; set; }

            public FactType IncrementCount()
            {
                TestCount++;
                return this;
            }

            public bool Equals(FactType other)
            {
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((FactType)obj);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        public class TestRule : BaseRule
        {
            public override void Define()
            {
                IEnumerable<FactType> facts = null;

                When()
                    .Query(() => facts, q => q
                        .Match<FactType>(f => f.Id != 0)
                        .GroupBy(f => f.GroupingProperty)
                        .Where(x => x.Select(xx => xx.Id).Distinct().Count() > 1)
                        .SelectMany(x => x)
                        .GroupBy(x => x.GroupingProperty2));
                Then()
                    .Do(ctx => Action(ctx));
            }
        }
    }
}
