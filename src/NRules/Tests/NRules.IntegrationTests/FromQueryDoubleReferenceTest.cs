using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class FromQueryDoubleReferenceTest : BaseRuleTestFixture
    {
        [Fact]
        public void FromDoubleReference_SplitByKey_FiresCorrectNumberOfTimesWithCorrectFactCounts()
        {
            // Arrange
            var values = new[] {"a", "a", "b", "b", "c", "c"};
            var keys = new[] {1, 2, 2, 1, 2, 3};
            var facts = values.Zip(keys, (v, k) => new Fact {Key = k, Value = v})
                .ToArray();

            Session.InsertAll(facts);

            // factsAll = 1a 2a 2b 1b 2c 3c
            // factsOne = 1a 1b
            // factsTwo = 2a 2b 2c

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            var firedFacts = GetFiredFacts<IEnumerable<Fact>>().ToArray();
            var factsAllExpected = facts;
            var factsOneExpected = facts.Where(f => f.Key == 1).ToArray();
            var factsTwoExpected = facts.Where(f => f.Key == 2).ToArray();
            var factsOneTwoExpected = factsOneExpected.Concat(factsTwoExpected).ToArray();
            var factsAllActual = firedFacts.Single(f => f.Count() == 6).ToArray();
            var factsOneActual = firedFacts.Single(f => f.Count() == 2).ToArray();
            var factsTwoActual = firedFacts.Single(f => f.Count() == 3).ToArray();
            var factsOneTwoActual = firedFacts.Single(f => f.Count() == 5).ToArray();

            Assert.Equal(factsAllExpected, factsAllActual);
            Assert.Equal(factsOneExpected, factsOneActual);
            Assert.Equal(factsTwoExpected, factsTwoActual);
            Assert.Equal(factsOneTwoExpected, factsOneTwoActual);
        }

        protected override void SetUpRules() { SetUpRule<FromQueryDoubleReferenceRule>(); }

        public class Fact
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }

        public class FromQueryDoubleReferenceRule : Rule
        {
            public override void Define()
            {
                IEnumerable<Fact> factsAll = null;
                IEnumerable<Fact> factsOne = null;
                IEnumerable<Fact> factsTwo = null;
                IEnumerable<Fact> factsOneTwo = null;

                When()
                    .Query(() => factsAll, q => q
                        .Match<Fact>()
                        .Collect()
                        .Where(c => c.Any()))
                    .Query(() => factsOne, q => q
                        .From(() => factsAll)
                        .SelectMany(f => f)
                        .Where(f => f.Key == 1)
                        .Collect()
                        .Where(c => c.Any()))
                    .Query(() => factsTwo, q => q
                        .From(() => factsAll)
                        .SelectMany(c => c)
                        .Where(f => f.Key == 2)
                        .Collect()
                        .Where(c => c.Any()))
                    .Query(() => factsOneTwo, q => q
                        .From(() => factsOne.Concat(factsTwo)));

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}