using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NRules.RuleModel;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class TwoFactOneExistsCheckRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_MatchingFacts_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingFactsInReverseOrder_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact2);
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingFacts_FactsInContext()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

            Session.Insert(fact1);
            Session.Insert(fact2);

            IFactMatch[] matches = null;
            GetRuleInstance<TwoFactOneExistsCheckRule>().Action = ctx =>
            {
                matches = ctx.Facts.ToArray();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.AreEqual(1, matches.Length);
            Assert.AreEqual("fact1", matches[0].Declaration.Name);
            Assert.AreSame(fact1, matches[0].Value);
        }

        [Test]
        public void Fire_MatchingFactsMultipleOfTypeTwo_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            var facts = new[] {fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_MatchingFactsTwoOfTypeOneMultipleOfTypeTwo_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            var fact3 = new FactType2 {TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value 4", JoinProperty = fact2.TestProperty};
            var fact5 = new FactType2 {TestProperty = "Valid Value 5", JoinProperty = fact2.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            var facts = new[] {fact3, fact4, fact5};
            Session.InsertAll(facts);


            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Test]
        public void Fire_FactOneValidFactTwoAssertedAndRetracted_DoesNotFire()
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
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactOneAssertedAndRetractedFactTwoValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Retract(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactOneAssertedAndUpdatedFactTwoValid_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Test]
        public void Fire_FactOneValidFactTwoAssertedAndUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Invalid Value 2";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_FactTwoDoesNotExist_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Test]
        public void Fire_TwoMatchingFactsAndOneFactExists_FiresOnce()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2"};
            var fact21 = new FactType2 {TestProperty = "Valid Value 3", JoinProperty = fact11.TestProperty};

            var facts = new[] {fact11, fact12};
            Session.InsertAll(facts);
            Session.Insert(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwoFactOneExistsCheckRule>();
        }
    }
}