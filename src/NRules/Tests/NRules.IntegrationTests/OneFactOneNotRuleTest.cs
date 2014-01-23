using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactOneNotRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingNotPatternFact_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }
        
        [Test]
        public void Fire_MatchingNotPatternFactAssertedThenRetracted_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};

            Session.Insert(fact1);
            Session.Retract(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_MatchingNotPatternFactAssertedThenUpdatedToInvalid_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};

            Session.Insert(fact1);

            fact1.TestProperty = "Invalid Value";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_NoFactMatchingNotPattern_FiresOnce()
        {
            //Arrange
            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactOneNotRule>();
        }
    }
}