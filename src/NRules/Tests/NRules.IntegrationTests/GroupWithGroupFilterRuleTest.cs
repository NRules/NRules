using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class GroupWithGroupFilterRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_TwoMatchingGroups_FiresTwice()
        {
            //Arrange
            var fact11 = new FactType {TestProperty = "Valid Test Property 1", GroupProperty = "GP1", GroupTestProperty = "Good"};
            var fact12 = new FactType {TestProperty = "Valid Test Property 1", GroupProperty = "GP1", GroupTestProperty = "Good" };
            var fact13 = new FactType {TestProperty = "Valid Test Property 1", GroupProperty = "GP2", GroupTestProperty = "Bad" };
            var fact14 = new FactType {TestProperty = "Valid Test Property 2", GroupProperty = "GP2", GroupTestProperty = "Good" };

            var facts = new object[] {fact11, fact12, fact13, fact14};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_MakeAllFactsInelligible_DoesNotFire()
        {
            //Arrange
            var fact11 = new FactType {TestProperty = "Valid Test Property 1", GroupProperty = "GP1", GroupTestProperty = "Good"};
            var fact12 = new FactType {TestProperty = "Valid Test Property 1", GroupProperty = "GP1", GroupTestProperty = "Good" };
            var fact13 = new FactType {TestProperty = "Valid Test Property 1", GroupProperty = "GP2", GroupTestProperty = "Bad" };
            var fact14 = new FactType {TestProperty = "Valid Test Property 2", GroupProperty = "GP2", GroupTestProperty = "Good" };

            var facts = new object[] {fact11, fact12, fact13, fact14};
            Session.InsertAll(facts);

            fact11.TestProperty = "Bad Test Poroperty";
            fact12.TestProperty = "Bad Test Poroperty";
            fact13.TestProperty = "Bad Test Poroperty";
            fact14.TestProperty = "Bad Test Poroperty";
            var factsToUpdate = new object[] { fact11, fact12, fact13, fact14 };
            Session.UpdateAll(factsToUpdate);

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
            public string GroupTestProperty { get; set; }
            public string GroupProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IGrouping<string, FactType> group = null;

                When()
                    .Query(() => group, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(f => f.GroupProperty)
                        .Where(z => HasCorrectValue(z)));
                Then()
                    .Do(ctx => ctx.NoOp());
            }

            private static bool HasCorrectValue(IGrouping<string, FactType> group)
            {
                return group.Any(x => x.GroupTestProperty.Contains("Good"));
            }
        }
    }
}
