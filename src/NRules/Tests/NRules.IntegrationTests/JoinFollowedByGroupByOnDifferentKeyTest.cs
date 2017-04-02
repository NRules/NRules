using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class JoinFollowedByGroupByOnDifferentKeyTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_BulkInsertForMultipleTypes_InsertsOnly_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact21 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
            var fact22 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

            var fact61 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact21.JoinProperty, GroupProperty = "Group1" };
            var fact62 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact21.JoinProperty, GroupProperty = "Group1" };
            var fact63 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact21.JoinProperty, GroupProperty = "Group2" };

            var fact64 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };
            var fact65 = new FactType2 { TestProperty = "Valid Group5", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };
            var fact66 = new FactType2 { TestProperty = "Valid Group6", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };

            var facts = new object[] { fact21, fact22, fact61, fact62, fact63, fact64, fact65, fact66 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            var firedFacts = new[]
            {
                GetFiredFact<IGrouping<string, FactType2>>(0),
                GetFiredFact<IGrouping<string, FactType2>>(1),
                GetFiredFact<IGrouping<string, FactType2>>(2)
            };

            var correctNumberofFactsPerGroup = firedFacts.Count(x => x.Count() == 1) == 1 &&
                                               firedFacts.Count(x => x.Count() == 2) == 1 &&
                                               firedFacts.Count(x => x.Count() == 3) == 1;
            Assert.IsTrue(correctNumberofFactsPerGroup);
        }

        [Test]
        public void Fire_BulkInsertForMultipleTypes_WithUpdates_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact21 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
            var fact22 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

            var fact61 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact21.JoinProperty, GroupProperty = "Group1" };
            var fact62 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact21.JoinProperty, GroupProperty = "Group1" };
            var fact63 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact21.JoinProperty, GroupProperty = "Group2" };

            var fact64 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };
            var fact65 = new FactType2 { TestProperty = "Valid Group5", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };
            var fact66 = new FactType2 { TestProperty = "Valid Group6", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };

            var facts = new object[] { fact21, fact22, fact61, fact62, fact63, fact64, fact65, fact66 };
            Session.InsertAll(facts);

            fact61.GroupProperty = "Group3";
            fact62.GroupProperty = "Group3";
            fact63.GroupProperty = "Group3";
            var factsForUpdate = new[] { fact61, fact62, fact63 };
            Session.UpdateAll(factsForUpdate);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(2);
            var firedFacts = new[]
            {
                GetFiredFact<IGrouping<string, FactType2>>(0),
                GetFiredFact<IGrouping<string, FactType2>>(1)
            };

            var correctNumberofFactsPerGroup = firedFacts.Count(x => x.Count() == 3) == 2;
            Assert.IsTrue(correctNumberofFactsPerGroup);
        }

        [Test]
        public void Fire_BulkInsertForMultipleTypes_WithRetracts_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact21 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Join 1" };
            var fact22 = new FactType1 { TestProperty = "Valid Value 2", JoinProperty = "Join 2" };

            var fact61 = new FactType2 { TestProperty = "Valid Group1", JoinProperty = fact21.JoinProperty, GroupProperty = "Group1" };
            var fact62 = new FactType2 { TestProperty = "Valid Group2", JoinProperty = fact21.JoinProperty, GroupProperty = "Group1" };
            var fact63 = new FactType2 { TestProperty = "Valid Group3", JoinProperty = fact21.JoinProperty, GroupProperty = "Group2" };

            var fact64 = new FactType2 { TestProperty = "Valid Group4", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };
            var fact65 = new FactType2 { TestProperty = "Valid Group5", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };
            var fact66 = new FactType2 { TestProperty = "Valid Group6", JoinProperty = fact22.JoinProperty, GroupProperty = "Group2" };

            var facts = new object[] { fact21, fact22, fact61, fact62, fact63, fact64, fact65, fact66 };
            Session.InsertAll(facts);
            
            var factsForRetract = new[] { fact61, fact62, fact63 };
            Session.RetractAll(factsForRetract);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(1);
            var firedFacts = new[]
            {
                GetFiredFact<IGrouping<string, FactType2>>(0)
            };

            var correctNumberofFactsPerGroup = firedFacts.Count(x => x.Count() == 3) == 1;
            Assert.IsTrue(correctNumberofFactsPerGroup);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
            public string GroupProperty { get; set; }
        }

        public class TestRule : BaseRule
        {
            public override void Define()
            {
                FactType1 fact = null;
                IGrouping<string, FactType2> group = null;

                When()
                    .Match<FactType1>(() => fact, f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => group, x => x
                        .Match<FactType2>(
                            f => f.TestProperty.StartsWith("Valid"),
                            f => f.JoinProperty == fact.JoinProperty)
                        .GroupBy(f => f.GroupProperty));
                Then()
                    .Do(ctx => Action(ctx));
            }
        }
    }
}
