using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class SelfRetractRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFact_FiresOnceAndRetractsFact()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var facts = new[] { fact1 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(0, Session.Query<FactType>().Count());
        }

        [Fact]
        public void Fire_NoMatchingFact_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Invalid Value 1" };
            var facts = new[] { fact1 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoMatchingFacts_FiresTwiceAndRetractsFacts()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Valid Value 2" };
            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(0, Session.Query<FactType>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string? TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType? fact = null;

                When()
                    .Match(() => fact, f => f!.TestProperty!.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.TryRetract(fact!));
            }
        }
    }
}