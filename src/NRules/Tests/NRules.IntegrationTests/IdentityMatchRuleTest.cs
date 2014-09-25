using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class IdentityMatchRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingFact_FiresOnce()
        {
            //Arrange
            var fact = new FactType1 {TestProperty = "Valid value"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_TwoMatchingFacts_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid value"};
            Session.Insert(fact1);
            var fact2 = new FactType1 {TestProperty = "Valid value"};
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }
        
        [Test]
        public void Fire_MatchingFactInsertedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact = new FactType1 { TestProperty = "Valid value" };
            Session.Insert(fact);
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_MatchingFactInsertedAndUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact = new FactType1 { TestProperty = "Valid value" };
            Session.Insert(fact);
            fact.TestProperty = "Invalid value";
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_NoMatchingFact_DoesNotFire()
        {
            //Arrange
            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<IdentityMatchRule>();
        }
    }
}