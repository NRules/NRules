using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class OneFactOneGroupByFlattenRuleTest : BaseRuleTestFixture
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
        public void Fire_TwoFactsForOneGroup_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value Group1"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupInsertedThenOneUpdated_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value Group1"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType1 {TestProperty = "Valid Value Group2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresWithEachFactFromEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType1 {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType1 {TestProperty = "Valid Value Group2"};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(4);
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
            Assert.AreEqual(fact3, GetFiredFact<FactType1>(2));
            Assert.AreEqual(fact4, GetFiredFact<FactType1>(3));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Valid Value Group2" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            Session.Retract(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresTwiceWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Valid Value Group2" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact4.TestProperty = "Invalid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresThreeTimesWithFactsFromGroupOne()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Valid Value Group2" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact4.TestProperty = "Valid Value Group1";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
            Assert.AreEqual(fact4, GetFiredFact<FactType1>(2));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresWithEachFactFromEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact3 = new FactType1 { TestProperty = "Valid Value Group2" };
            var fact4 = new FactType1 { TestProperty = "Invalid Value" };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            fact4.TestProperty = "Valid Value Group2";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(4);
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
            Assert.AreEqual(fact3, GetFiredFact<FactType1>(2));
            Assert.AreEqual(fact4, GetFiredFact<FactType1>(3));
        }

        [Test]
        public void Fire_TwoFactsForOneGroupAssertedThenOneRetractedAnotherUpdatedThenOneAssertedBack_FiresTwiceWithFactsFromOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value Group1" };
            var fact2 = new FactType1 { TestProperty = "Valid Value Group1" };

            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Retract(fact2);
            
            Session.Update(fact1);
            Session.Insert(fact2);
            
            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(2);
            Assert.AreEqual(fact1, GetFiredFact<FactType1>(0));
            Assert.AreEqual(fact2, GetFiredFact<FactType1>(1));
        }

        protected override void SetUpRules()
        {
            SetUpRule<OneFactOneGroupByFlattenRule>();
        }
    }
}