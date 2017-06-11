using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class HaltRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_TwoMatchingFacts_FiresOnceAndHalts()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Fact]
        public void Fire_TwoMatchingFactsFireCalledTwice_FiresOnceThenHaltsThenResumesAndFiresAgain()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();
            Session.Fire();

            //Assert
            AssertFiredTwice();
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
                    .Do(ctx => ctx.Halt());
            }
        }
    }
}