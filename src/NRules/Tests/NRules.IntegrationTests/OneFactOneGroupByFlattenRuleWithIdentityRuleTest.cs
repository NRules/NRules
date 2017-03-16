using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
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
                new FactType7 {Id = 1, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group"},
                new FactType7 {Id = 2, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
                new FactType7 {Id = 3, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group3"},
                new FactType7 {Id = 4, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group2"}
            };
            var factsToUpdate = new object[]
            {
                new FactType7 {Id = 1, TestCount = 2, GroupingProperty = "GP1", GroupingProperty2 = "Group"},
                new FactType7 {Id = 2, TestCount = 3, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
                new FactType7 {Id = 3, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
                new FactType7 {Id = 4, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group"}
            };
            Session.InsertAll(factsToInsert);
            Session.UpdateAll(factsToUpdate);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedFacts = GetFiredFact<IGrouping<string, FactType7>>();
            Assert.AreEqual(4, firedFacts.Count());
            Assert.AreEqual(1, firedFacts.Count(x => x.TestCount == 3));
            Assert.AreEqual(1, firedFacts.Count(x => x.TestCount == 2));
            Assert.AreEqual(2, firedFacts.Count(x => x.TestCount == 1));
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactOneGroupByFlattenRuleWithIdentityRule>();
        }
    }
}
