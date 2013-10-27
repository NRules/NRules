using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneExistsCheckRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void OneFactOneExistsCheckRule_MatchingFacts_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void OneFactOneExistsCheckRule_MatchingFactsMultipleOfTypeTwo_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact3 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void OneFactOneExistsCheckRule_MatchingFactsTwoOfTypeOneMultipleOfTypeTwo_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value One"};
            var fact2 = new FactType1() {TestProperty = "Valid Value Two"};
            var fact3 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};
            var fact4 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact2};
            var fact5 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact2};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);
            Session.Insert(fact5);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void OneFactOneExistsCheckRule_FactOneValidFactTwoAssertedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value", JoinReference = fact1};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void OneFactOneExistsCheckRule_FactOneValidFactTwoAssertedAndUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};
            var fact2 = new FactType2() {TestProperty = "Valid Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Invalid Value";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void OneFactOneExistsCheckRule_FactTwoDoesNotExist_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1() {TestProperty = "Valid Value"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneExistsCheckRule>();
        }
    }
}