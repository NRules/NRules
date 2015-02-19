using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOrGroupRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            //Arrange
            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactMatchingFirstPartOfOrGroup_FiresOnce()
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
        public void Fire_FactsMatchingSecondPartOfOrGroup_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_FactsMatchingBothPartsOfOrGroup_FiresTwice()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact12 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact12.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOrGroupRule>();
        }
    }
}