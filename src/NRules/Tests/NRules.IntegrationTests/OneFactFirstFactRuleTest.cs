using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactFirstFactRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_FiveMatchingFactsInserted_FiresOnceWithFirstFact()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 3 };
            var fact2 = new FactType { TestProperty = 5 };
            var fact3 = new FactType { TestProperty = 1 };
            var fact4 = new FactType { TestProperty = 4 };
            var fact5 = new FactType { TestProperty = 2 };

            var facts = new[] { fact1, fact2, fact3, fact4, fact5 };
            Session.InsertAll(facts);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();
            Assert.Equal(fact3, GetFiredFact<FactType>());
        }


        [Fact]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            // Arrange 
            var fact = new FactType { TestProperty = 0 };
            Session.Insert(fact);

            // Act
            Session.Fire();

            // Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithFirstFact()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 0 };
            var fact2 = new FactType { TestProperty = 10 };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            fact1.TestProperty = 5;
            Session.Update(fact1);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();
            Assert.Equal(fact1, GetFiredFact<FactType>());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithFirstFact()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 10 };
            var fact2 = new FactType { TestProperty = 20 };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);
            Session.Retract(fact2);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();
            Assert.Equal(fact1, GetFiredFact<FactType>());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public int TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType fact = null;

                When()
                    .Query(() => fact, x => x
                        .Match<FactType>(f => f.TestProperty > 0)
                        .Collect()
                        .OrderBy(f => f.TestProperty)
                        .Where(f => f.Any())
                        .Select(f => f.First()));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}