using System.Collections.Generic;
using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactOneCollectionRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_NoMatchingFacts_FiresOnceWithEmptyCollection()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(0, GetFiredFact<IEnumerable<FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            var fact3 = new FactType1 {TestProperty = "Invalid Value 3"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IEnumerable<FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, GetFiredFact<IEnumerable<FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(0, GetFiredFact<IEnumerable<FactType1>>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactOneCollectionRule>();
        }
    }
}