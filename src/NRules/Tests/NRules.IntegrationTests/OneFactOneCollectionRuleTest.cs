using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactOneCollectionRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_NoMatchingFacts_FiresOnceWithEmptyCollection()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};
            var fact3 = new FactType {TestProperty = "Invalid Value 3"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Retract(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            fact2.TestProperty = "Invalid Value";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoFactInCollection()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Invalid Value"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            fact2.TestProperty = "Valid Value 2";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
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
                IEnumerable<FactType> collection = null;

                When()
                    .Query(() => collection, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .Collect());
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}