using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactSimpleGroupByRuleTest : BaseRuleTestFixture
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
        public void Fire_TwoFactsWithNullKey_FiresOnceWithBothFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = null};
            var fact2 = new FactType1 {TestProperty = null};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(2, GetFiredFact<IGrouping<string, FactType1>>().Count());
        }

        [Test]
        public void Fire_TwoFactsWithNullKeyOneKeyUpdatedToValue_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = null};
            var fact2 = new FactType1 {TestProperty = null};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Value";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType1>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType1>>(1).Count());
        }

        [Test]
        public void Fire_TwoFactsWithValueKeyOneKeyUpdatedToNull_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Value"};
            var fact2 = new FactType1 {TestProperty = "Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = null;
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType1>>(0).Count());
            Assert.AreEqual(1, GetFiredFact<IGrouping<string, FactType1>>(1).Count());
        }

        [Test]
        public void Fire_TwoFactsWithNullBothRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = null};
            var fact2 = new FactType1 {TestProperty = null};

            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Retract(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactSimpleGroupByRule>();
        }
    }
}