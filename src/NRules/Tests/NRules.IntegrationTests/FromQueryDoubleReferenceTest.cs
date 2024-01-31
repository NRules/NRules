using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class FromQueryDoubleReferenceTest : BaseRulesTestFixture
{
    [Fact]
    public void FromDoubleReference_SplitByKey_FiresCorrectNumberOfTimesWithCorrectFactCounts()
    {
        // Arrange
        var values = new[] { "a", "a", "b", "b", "c", "c" };
        var keys = new[] { 1, 2, 2, 1, 2, 3 };
        var facts = values.Zip(keys, (v, k) => new Fact { Key = k, Value = v })
            .ToArray();

        Session.InsertAll(facts);

        // factsAll = 1a 2a 2b 1b 2c 3c
        // factsOne = 1a 1b
        // factsTwo = 2a 2b 2c

        // Act
        Session.Fire();

        // Assert
        IEnumerable<Fact> factsAllActual = null!;
        IEnumerable<Fact> factsOneActual = null!;
        IEnumerable<Fact> factsTwoActual = null!;
        IEnumerable<Fact> factsOneTwoActual = null!;
        Verify(x => x.Rule().Fired(
            Matched.Fact<IEnumerable<Fact>>()
                .Callback(f => factsAllActual = f),
            Matched.Fact<IEnumerable<Fact>>()
                .Callback(f => factsOneActual = f),
            Matched.Fact<IEnumerable<Fact>>()
                .Callback(f => factsTwoActual = f),
            Matched.Fact<IEnumerable<Fact>>()
                .Callback(f => factsOneTwoActual = f)));

        var factsAllExpected = facts;
        var factsOneExpected = facts.Where(f => f.Key == 1).ToArray();
        var factsTwoExpected = facts.Where(f => f.Key == 2).ToArray();
        var factsOneTwoExpected = factsOneExpected.Concat(factsTwoExpected).ToArray();

        Assert.Equal(factsAllExpected, factsAllActual);
        Assert.Equal(factsOneExpected, factsOneActual);
        Assert.Equal(factsTwoExpected, factsTwoActual);
        Assert.Equal(factsOneTwoExpected, factsOneTwoActual);
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<FromQueryDoubleReferenceRule>();
    }

    public class Fact
    {
        public int Key { get; set; }
        [NotNull]
        public string? Value { get; set; }
    }

    public class FromQueryDoubleReferenceRule : Rule
    {
        public override void Define()
        {
            IEnumerable<Fact> factsAll = null!;
            IEnumerable<Fact> factsOne = null!;
            IEnumerable<Fact> factsTwo = null!;
            IEnumerable<Fact> factsOneTwo = null!;

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