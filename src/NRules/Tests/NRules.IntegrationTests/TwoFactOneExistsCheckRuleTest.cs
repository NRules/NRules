using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactOneExistsCheckRuleTest : BaseRuleTestFixture
    {
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

        [Fact]
        public void Fire_MatchingFacts_FactsInContext()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty };

            Session.Insert(fact1);
            Session.Insert(fact2);

            IFactMatch[] matches = null;
            GetRuleInstance<TestRule>().Action = ctx =>
            {
                matches = ctx.Facts.ToArray();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, matches.Length);
            Assert.Equal("fact", matches[0].Declaration.Name);
            Assert.Same(fact1, matches[0].Value);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void Fire_TwoMatchingSetsFactOfSecondKindUpdated_DoesNotRefire()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value"};
            var fact12 = new FactType1 {TestProperty = "Valid Value"};
            var fact21 = new FactType2 {TestProperty = "Valid Value", JoinProperty = "Valid Value"};

            var facts = new[] {fact11, fact12};
            Session.InsertAll(facts);
            Session.Insert(fact21);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredTwice();

            //Act - 2
            Session.Update(fact21);
            Session.Fire();

            //Assert - 2
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_TwoMatchingSetsFactOfSecondKindRetractedThenInserted_FiresTwiceThenFiresTwice()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value"};
            var fact12 = new FactType1 {TestProperty = "Valid Value"};
            var fact21 = new FactType2 {TestProperty = "Valid Value", JoinProperty = "Valid Value"};

            var facts = new[] {fact11, fact12};
            Session.InsertAll(facts);
            Session.Insert(fact21);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredTwice();

            //Act - 2
            Session.Retract(fact21);
            Session.Insert(fact21);
            Session.Fire();

            //Assert - 2
            AssertFiredTimes(4);
        }

        [Fact]
        public void Fire_TwoMatchingSetsFactOfFirstKindUpdated_FiresTwiceThenFiresOnce()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value"};
            var fact12 = new FactType1 {TestProperty = "Valid Value"};
            var fact21 = new FactType2 {TestProperty = "Valid Value", JoinProperty = "Valid Value"};

            var facts = new[] {fact11, fact12};
            Session.InsertAll(facts);
            Session.Insert(fact21);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredTwice();

            //Act - 2
            Session.Update(fact11);
            Session.Fire();

            //Assert - 2
            AssertFiredTimes(3);
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
            public Action<IContext> Action = ctx => { };

            public override void Define()
            {
                FactType1 fact = null;

                When()
                    .Match<FactType1>(() => fact, f => f.TestProperty.StartsWith("Valid"))
                    .Exists<FactType2>(f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact.TestProperty);
                Then()
                    .Do(ctx => Action(ctx));
            }
        }
    }
}