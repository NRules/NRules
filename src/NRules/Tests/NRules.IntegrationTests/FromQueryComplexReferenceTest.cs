using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class FromQueryComplexReferenceTest : BaseRulesTestFixture
{
    [Fact]
    public void FromComplex_JoinedWithKeyAndFactsFiltered_FiresForEachGroup()
    {
        // Arrange
        var values = new[] { "a", "b", "b", "a" };
        var keys = new[] { 1, 1, 2, 2 };
        var facts = values.Zip(keys, (v, k) => new Fact { Key = k, Value = v });

        var key = new KeyFact { Value = 1 };

        Session.Insert(key);
        Session.InsertAll(facts);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Twice));
    }

    [Fact]
    public void FromComplex_JoinedWithKeyAndFactsFiltered_NoMatches_DoesNotFire()
    {
        // Arrange
        var values = new[] { "a", "b", "b", "a" };
        var keys = new[] { 1, 1, 2, 2 };
        var facts = values.Zip(keys, (v, k) => new Fact { Key = k, Value = v });

        var key = new KeyFact { Value = 3 };

        Session.Insert(key);
        Session.InsertAll(facts);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<FromQueryComplexReferenceRule>();
    }

    public class Fact
    {
        public int Key { get; set; }
        [NotNull]
        public string? Value { get; set; }
    }

    public class KeyFact
    {
        public int Value { get; set; }
    }

    public class FromQueryComplexReferenceRule : Rule
    {
        public override void Define()
        {
            IEnumerable<Fact> factsAll = null!;
            KeyFact key = null!;
            IGrouping<string, Fact> factsFilteredGrouped = null!;

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