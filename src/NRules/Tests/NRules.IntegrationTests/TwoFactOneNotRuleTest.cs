using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneNotRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingNotPatternFacts_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }
        
        [Test]
        public void Fire_MatchingNotPatternFactAssertedThenRetracted_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Test]
        public void Fire_MatchingNotPatternFactAssertedThenUpdatedToInvalid_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Invalid Value 2";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingFactAndNoFactsMatchingNotPattern_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_TwoMatchingFactsAndNoFactsMatchingNotPattern_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void Fire_TwoMatchingFactsAndOneFactMatchingNotPattern_FiresOnce()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
            var fact21 = new FactType2 { TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneNotRule>();
        }
    }
}