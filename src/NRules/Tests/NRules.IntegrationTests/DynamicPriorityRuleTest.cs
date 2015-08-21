using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class DynamicPriorityRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingFactsWithIncreasingPriority_FiresInReverseOrder()
        {
            //Arrange
            var fact1 = new FactType5 {TestProperty = "Valid Value 1", TestCount = 1};
            var fact2 = new FactType5 {TestProperty = "Valid Value 2", TestCount = 2};
            var fact3 = new FactType5 {TestProperty = "Valid Value 3", TestCount = 3};
            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            Assert.AreEqual(3, GetFiredFact<FactType5>(0).TestCount);
            Assert.AreEqual(2, GetFiredFact<FactType5>(1).TestCount);
            Assert.AreEqual(1, GetFiredFact<FactType5>(2).TestCount);
        }

        [Test]
        public void Fire_MatchingFactsWithDecreasingPriority_FiresInDirectOrder()
        {
            //Arrange
            var fact1 = new FactType5 {TestProperty = "Valid Value 1", TestCount = 3};
            var fact2 = new FactType5 {TestProperty = "Valid Value 2", TestCount = 2};
            var fact3 = new FactType5 {TestProperty = "Valid Value 3", TestCount = 1};
            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            Assert.AreEqual(3, GetFiredFact<FactType5>(0).TestCount);
            Assert.AreEqual(2, GetFiredFact<FactType5>(1).TestCount);
            Assert.AreEqual(1, GetFiredFact<FactType5>(2).TestCount);
        }
        
        protected override void SetUpRules()
        {
            SetUpRule<DynamicPriorityRule>();
        }
    }
}