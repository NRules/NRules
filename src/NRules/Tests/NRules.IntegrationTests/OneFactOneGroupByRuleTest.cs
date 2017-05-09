using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactOneGroupByRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>(1).Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            Session.Retract(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Invalid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresOnceWithThreeFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Valid Value Group2"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Valid Value Group1";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var actual = GetFiredFact<IGrouping<string, string>>().Count();
            Assert.Equal(3, actual);
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value Group1"};
            var fact2 = new FactType {TestProperty = "Valid Value Group1"};
            var fact3 = new FactType {TestProperty = "Valid Value Group2"};
            var fact4 = new FactType {TestProperty = "Invalid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Valid Value Group2";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<string, string>>(1).Count());
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
                IGrouping<string, string> group = null;

                When()
                    .Query(() => group, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(f => f.TestProperty, f => f.TestProperty)
                        .Where(g => g.Count() > 1));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}