using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class FromQueryComplexReferenceTest : BaseRuleTestFixture
    {
        [Fact]
        public void FromComplex_JoinedWithKeyAndFactsFiltered_FiresForEachGroup()
        {
            // Arrange
            var values = new[] {"a", "b", "b", "a"};
            var keys = new[] {1, 1, 2, 2};
            var facts = values.Zip(keys, (v, k) => new Fact {Key = k, Value = v});

            var key = new Key {Value = 1};

            Session.Insert(key);
            Session.InsertAll(facts);

            // Act
            Session.Fire();

            // Assert
            AssertFiredTwice();
        }

        [Fact]
        public void FromComplex_JoinedWithKeyAndFactsFiltered_NoMatches_DoesNotFire()
        {
            // Arrange
            var values = new[] {"a", "b", "b", "a"};
            var keys = new[] {1, 1, 2, 2};
            var facts = values.Zip(keys, (v, k) => new Fact {Key = k, Value = v});

            var key = new Key {Value = 3};

            Session.Insert(key);
            Session.InsertAll(facts);

            // Act
            Session.Fire();

            // Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<FromQueryComplexReferenceRule>();
        }

        public class Fact
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }

        public class Key
        {
            public int Value { get; set; }
        }

        public class FromQueryComplexReferenceRule : Rule
        {
            public override void Define()
            {
                IEnumerable<Fact> factsAll = null;
                Key key = null;
                IGrouping<string, Fact> factsFilteredGrouped = null;

                When()
                    .Query(() => factsAll, q => q
                        .Match<Fact>()
                        .Collect()
                        .Where(c => c.Any()))
                    .Match(() => key)
                    .Query(() => factsFilteredGrouped, q => q
                        .From(() => factsAll.Where(f => f.Key == key.Value))
                        .SelectMany(c => c)
                        .GroupBy(f => f.Value));

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}