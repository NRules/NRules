using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ForwardChainingLinkedTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFact_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1" };
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredOnce<ForwardChainingSecondRule>();
        }

        [Fact]
        public void Fire_OneMatchingFactInsertedThenUpdated_FiresFirstRuleAndChainsSecondLinkedFactUpdated()
        {
            //Arrange
            FactType2 matchedFact = null;
            Session.Events.FactInsertedEvent += (sender, args) =>
            {
                if (args.Fact.Type == typeof(FactType2)) matchedFact = (FactType2) args.Fact.Value;
            };
            Session.Events.FactUpdatedEvent += (sender, args) =>
            {
                if (args.Fact.Type == typeof(FactType2)) matchedFact = (FactType2) args.Fact.Value;
            };

            var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1" };
            Session.Insert(fact1);

            //Act - I
            Session.Fire();

            //Assert - I
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredOnce<ForwardChainingSecondRule>();
            Assert.Equal(1, matchedFact.Count);

            //Act - II
            Session.Update(fact1);
            Session.Fire();

            //Assert - II
            AssertFiredTwice<ForwardChainingFirstRule>();
            AssertFiredTwice<ForwardChainingSecondRule>();
            Assert.Equal(2, matchedFact.Count);
        }

        [Fact]
        public void Fire_OneMatchingFactInsertedThenRetracted_FiresFirstRuleAndChainsSecondLinkedFactRetracted()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1" };
            Session.Insert(fact1);

            //Act - I
            Session.Fire();

            //Assert - I
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredOnce<ForwardChainingSecondRule>();
            Assert.Equal(1, Session.Query<FactType2>().Count());

            //Act - II
            Session.Retract(fact1);

            //Assert - II
            Assert.Equal(0, Session.Query<FactType2>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<ForwardChainingFirstRule>();
            SetUpRule<ForwardChainingSecondRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class FactType2
        {
            public int Count { get; set; } = 1;
            public string TestProperty { get; set; }
        }

        public class ForwardChainingFirstRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Yield(ctx => new FactType2 {TestProperty = fact1.JoinProperty}, (ctx, fact2) => Update(fact1, fact2));
            }

            private static FactType2 Update(FactType1 fact1, FactType2 fact2)
            {
                fact2.TestProperty = fact1.JoinProperty;
                fact2.Count++;
                return fact2;
            }
        }

        public class ForwardChainingSecondRule : Rule
        {
            public override void Define()
            {
                FactType2 fact2 = null;

                When()
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}