using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactSameTypeRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_MatchingFacts_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2", Parent = fact1};
            var fact3 = new FactType {TestProperty = "Invalid Value 3", Parent = fact1};
            var fact4 = new FactType {TestProperty = "Valid Value 4", Parent = null};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Fact]
        public void Fire_FirstMatchingFactSecondInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
            public FactType Parent { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType fact1 = null;
                FactType fact2 = null;

                When()
                    .Match<FactType>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match<FactType>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.Parent == fact1);

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}