using System.Collections.Generic;
using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactAggregateJoinRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_OneMatchingFact_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetFiredFact<IEnumerable<FactType1>>().Count());
        }

        [Test]
        public void Fire_OneMatchingFactTwoFactsToAggregate_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Invalid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IEnumerable<FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsTwoFactsToAggregate_FiresTwiceWithTwoFactsInEachCollection()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(2, GetFiredFact<IEnumerable<FactType1>>(0).Count());
            Assert.AreEqual(2, GetFiredFact<IEnumerable<FactType1>>(1).Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactAggregateJoinRule>();
        }
    }
}
