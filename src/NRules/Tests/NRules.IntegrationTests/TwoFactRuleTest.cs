using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_MatchingFactsInsertAndFireThenUpdateAndFire_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();
            Session.Update(fact1);
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }
        
        [Fact]
        public void Fire_MatchingFactsInsertThenUpdateThenFire_FiresOnce()
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
        
        [Fact]
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

        [Fact]
        public void Fire_MatchingFactsReverseOrder_FiresOnce()
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

        [Fact]
        public void Fire_TwoMatchingPairsOfFacts_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            var fact3 = new FactType1 {TestProperty = "Valid Value 3"};
            var fact4 = new FactType2 {TestProperty = "Valid Value 4", JoinProperty = fact3.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_TwoMatchingFactsOfOneKindOneMatchingFactOfAnotherKind_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_FirstMatchingFactSecondInvalidFact_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FirstInvalidFactSecondMatchingFact_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoMatchingFactsUnsatisfiedJoin_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = null};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoMatchingFactsFirstRetracted_DoesNotFire()
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

        [Fact]
        public void Fire_TwoMatchingFactsSecondRetracted_DoesNotFire()
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

        [Fact]
        public void Fire_TwoMatchingFactsFirstUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact1.TestProperty = "Invalid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneInvalidFactAndSecondMatchingFactFirstUpdatedToValid_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact1.TestProperty = "Valid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_TwoMatchingFactsSecondUpdatedToInvalid_DoesNotFire()
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
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneMatchingFactSecondInvalidThenUpdatedToValid_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Valid Value 2";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_OneMatchingFactSecondInvalidThenUpdatedToValidJoin_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = null};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.JoinProperty = fact1.TestProperty;
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public FactType1 Fact1 { get; set; }
            public FactType2 Fact2;

            public override void Define()
            {
                When()
                    .Match<FactType1>(() => Fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match<FactType2>(() => Fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == Fact1.TestProperty);

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}