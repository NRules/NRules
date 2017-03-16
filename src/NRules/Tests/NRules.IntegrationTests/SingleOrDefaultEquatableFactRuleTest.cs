using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class SingleOrDefaultEquatableFactRuleTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_OneMatchingFactsAndOneInvalid_FiresOnceWithValidFact()
        {
            //Arrange
            var fact1 = new EquatableFact(1) { TestProperty = "Invalid Value 1", ValueProperty = "Original 1"};
            var fact2 = new EquatableFact(2) { TestProperty = "Valid Value 2", ValueProperty = "Original 2"};

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedFact = GetFiredFact<EquatableFact>();
            Assert.AreEqual(2, firedFact.Id);
            Assert.AreEqual("Original 2", firedFact.ValueProperty);
        }
        
        [Test]
        public void Fire_NoValidFacts_FiresOnceWithDefault()
        {
            //Arrange
            var fact1 = new EquatableFact(1) { TestProperty = "Invalid Value 1", ValueProperty = "Original 1"};

            var facts = new[] { fact1 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedFact = GetFiredFact<EquatableFact>();
            Assert.AreEqual(0, firedFact.Id);
            Assert.Null(firedFact.ValueProperty);
        }
        
        [Test]
        public void Fire_NoValidFactsUpdatedToValid_FiresOnceWithValidFact()
        {
            //Arrange
            var fact1 = new EquatableFact(1) { TestProperty = "Invalid Value 1", ValueProperty = "Original 1"};

            var facts = new[] { fact1 };
            Session.InsertAll(facts);

            var fact11 = new EquatableFact(1) { TestProperty = "Valid Value 1", ValueProperty = "Original 1" };
            Session.Update(fact11);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedFact = GetFiredFact<EquatableFact>();
            Assert.AreEqual(1, firedFact.Id);
            Assert.AreEqual("Original 1", firedFact.ValueProperty);
        }
        
        [Test]
        public void Fire_ValidFactInsertedThenUpdated_FiresOnceWithUpdatedValue()
        {
            //Arrange
            var fact1 = new EquatableFact(1) { TestProperty = "Valid Value 1", ValueProperty = "Original 1"};

            var facts = new[] { fact1 };
            Session.InsertAll(facts);

            var fact11 = new EquatableFact(1) { TestProperty = "Valid Value 1", ValueProperty = "Updated 1" };
            Session.Update(fact11);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var firedFact = GetFiredFact<EquatableFact>();
            Assert.AreEqual(1, firedFact.Id);
            Assert.AreEqual("Updated 1", firedFact.ValueProperty);
        }

        protected override void SetUpRules()
        {
            SetUpRule<SingleOrDefaultEquatableFactRule>();
        }
    }
}