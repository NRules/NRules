using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactOneSortedDescendingCollectionRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_NoMatchingFacts_FiresOnceWithEmptyCollection()
        {
            // Arrange - Act
            Session.Fire();

            // Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoSortedFactsInCollection()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 0 };
            var fact2 = new FactType { TestProperty = 10 };
            var fact3 = new FactType { TestProperty = 5 };

            var facts = new[] { fact1, fact2, fact3 };
            Session.InsertAll(facts);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            var firedFacts = GetFiredFact<IEnumerable<FactType>>();
            Assert.Equal(2, firedFacts.Count());
            Assert.Equal(fact2, firedFacts.ElementAt(0));
            Assert.Equal(fact3, firedFacts.ElementAt(1));
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoSortedFactsInCollection()
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

            var firedFacts = GetFiredFact<IEnumerable<FactType>>();
            Assert.Equal(2, firedFacts.Count());
            Assert.Equal(fact2, firedFacts.ElementAt(0));
            Assert.Equal(fact1, firedFacts.ElementAt(1));
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 10 };
            var fact2 = new FactType { TestProperty = 5 };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);
            Session.Retract(fact2);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            var firedFacts = GetFiredFact<IEnumerable<FactType>>();
            Assert.Equal(1, firedFacts.Count());
            Assert.Equal(fact1, firedFacts.ElementAt(0));
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 10 };
            var fact2 = new FactType { TestProperty = 5 };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);
            Session.Retract(fact1);
            Session.Retract(fact2);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 10 };
            var fact2 = new FactType { TestProperty = 5 };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            fact2.TestProperty = 0;
            Session.Update(fact2);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            var firedFacts = GetFiredFact<IEnumerable<FactType>>();
            Assert.Equal(1, firedFacts.Count());
            Assert.Equal(fact1, firedFacts.ElementAt(0));
        }

        [Fact]
        public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoSortedFactInCollection()
        {
            // Arrange
            var fact1 = new FactType { TestProperty = 2 };
            var fact2 = new FactType { TestProperty = 0 };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            fact2.TestProperty = 1;
            Session.Update(fact2);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            var firedFacts = GetFiredFact<IEnumerable<FactType>>();
            Assert.Equal(2, firedFacts.Count());
            Assert.Equal(fact1, firedFacts.ElementAt(0));
            Assert.Equal(fact2, firedFacts.ElementAt(1));
        }

        [Fact]
        public void Fire_FiveMatchingFactsInserted_FiresOnceWithFiveSortedFactsInCollection()
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

            var firedFacts = GetFiredFact<IEnumerable<FactType>>();
            Assert.Equal(5, firedFacts.Count());
            Assert.Equal(fact2, firedFacts.ElementAt(0));
            Assert.Equal(fact4, firedFacts.ElementAt(1));
            Assert.Equal(fact1, firedFacts.ElementAt(2));
            Assert.Equal(fact5, firedFacts.ElementAt(3));
            Assert.Equal(fact3, firedFacts.ElementAt(4));
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
                IEnumerable<FactType> collection = null;

                When()
                    .Query(() => collection, x => x
                        .Match<FactType>(f => f.TestProperty > 0)
                        .Collect()
                        .OrderByDescending(f => f.TestProperty));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}