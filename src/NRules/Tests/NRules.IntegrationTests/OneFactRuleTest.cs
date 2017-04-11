using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFact_FiresOnce()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_TwoMatchingFacts_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_ConditionDoesNotMatch_DoesNotFire()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Invalid Value 1"};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneMatchingFactAssertedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneFactUpdatedFromInvalidToMatching_FiresOnce()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Invalid Value 1"};
            Session.Insert(fact);

            fact.TestProperty = "Valid Value 1";
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_OneMatchingFactAssertedAndRetractedAndAssertedAgain_FiresOnce()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);
            Session.Retract(fact);
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_OneMatchingFactAssertedAndUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            fact.TestProperty = "Invalid Value 1";
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneMatchingFactAssertedAndModifiedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};
            Session.Insert(fact);

            fact.TestProperty = "Invalid Value 1";
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Insert_Null_Throws()
        {
            //Arrange - Act - Assert
            Assert.Throws<ArgumentNullException>(() => Session.Insert(null));
        }

        [Fact]
        public void Insert_DuplicateInsert_Throws()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};

            //Act - Assert
            Session.Insert(fact);
            Assert.Throws<ArgumentException>(() => Session.Insert(fact));
        }

        [Fact]
        public void TryInsert_DuplicateInsert_False()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};

            //Act
            Session.Insert(fact);
            bool actual = Session.TryInsert(fact);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void Update_Null_Throws()
        {
            //Arrange - Act - Assert
            Assert.Throws<ArgumentNullException>(() => Session.Update(null));
        }

        [Fact]
        public void Update_UpdateWithoutInsert_Throws()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};

            //Act - Assert
            Assert.Throws<ArgumentException>(() => Session.Update(fact));
        }

        [Fact]
        public void TryUpdate_UpdateWithoutInsert_False()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};

            //Act
            bool actual = Session.TryUpdate(fact);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void Retract_Null_Throws()
        {
            //Arrange - Act - Assert
            Assert.Throws<ArgumentNullException>(() => Session.Retract(null));
        }

        [Fact]
        public void Retract_RetractWithoutInsert_Throws()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};

            //Act - Assert
            Assert.Throws<ArgumentException>(() => Session.Retract(fact));
        }

        [Fact]
        public void TryRetract_RetractWithoutInsert_False()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1"};

            //Act
            bool actual = Session.TryRetract(fact);

            //Assert
            Assert.False(actual);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType fact = null;

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}