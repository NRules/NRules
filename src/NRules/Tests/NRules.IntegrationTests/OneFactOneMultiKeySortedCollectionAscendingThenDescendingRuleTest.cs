﻿using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneMultiKeySortedCollectionDescendingThenAscendingRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_NoMatchingFacts_FiresOnceWithEmptyCollection()
    {
        // Arrange - Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>(f => !f.Any())));
    }

    [Fact]
    public void Fire_FourMatchingFactsAndOneInvalid_FiresOnceWithFourSortedFactsInCollection()
    {
        // Arrange
        var fact1 = new FactType(0, null);
        var fact2 = new FactType(50, "A");
        var fact3 = new FactType(10, "A");
        var fact4 = new FactType(50, "B");
        var fact5 = new FactType(10, "B");

        var facts = new[] { fact1, fact2, fact3, fact4, fact5 };
        Session.InsertAll(facts);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>()
            .Callback(firedFact =>
            {
                Assert.Equal(4, firedFact.Count());
                Assert.Equal(fact2, firedFact.ElementAt(0));
                Assert.Equal(fact4, firedFact.ElementAt(1));
                Assert.Equal(fact3, firedFact.ElementAt(2));
                Assert.Equal(fact5, firedFact.ElementAt(3));
            })));
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoSortedFactsInCollection()
    {
        // Arrange
        var fact1 = new FactType(0, "B");
        var fact2 = new FactType(10, "A");

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        fact1.TestPropertyInt = 10;
        Session.Update(fact1);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>()
            .Callback(firedFact =>
            {
                Assert.Equal(2, firedFact.Count());
                Assert.Equal(fact2, firedFact.ElementAt(0));
                Assert.Equal(fact1, firedFact.ElementAt(1));
            })));
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
    {
        // Arrange
        var fact1 = new FactType(5, "A");
        var fact2 = new FactType(10, "A");

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);
        Session.Retract(fact2);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>()
            .Callback(firedFact =>
            {
                Assert.Single(firedFact);
                Assert.Equal(fact1, firedFact.ElementAt(0));
            })));
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
    {
        // Arrange
        var fact1 = new FactType(5, "A");
        var fact2 = new FactType(10, "A");

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);
        Session.Retract(fact1);
        Session.Retract(fact2);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>(x => !x.Any())));
    }

    [Fact]
    public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
    {
        // Arrange
        var fact1 = new FactType(5, "A");
        var fact2 = new FactType(10, "A");

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        fact2.TestPropertyInt = 0;
        Session.Update(fact2);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>()
            .Callback(firedFact =>
            {
                Assert.Single(firedFact);
                Assert.Equal(fact1, firedFact.ElementAt(0));
            })));
    }

    [Fact]
    public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoSortedFactInCollection()
    {
        // Arrange
        var fact1 = new FactType(2, "A");
        var fact2 = new FactType(0, "B");

        var facts = new[] { fact1, fact2 };
        Session.InsertAll(facts);

        fact2.TestPropertyInt = 2;
        Session.Update(fact2);

        // Act
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>()
            .Callback(firedFact =>
            {
                Assert.Equal(2, firedFact.Count());
                Assert.Equal(fact1, firedFact.ElementAt(0));
                Assert.Equal(fact2, firedFact.ElementAt(1));
            })));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        public FactType(int testInt, string? testString)
        {
            TestPropertyInt = testInt;
            TestPropertyString = testString;
        }

        public int TestPropertyInt { get; set; }
        public string? TestPropertyString { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            IEnumerable<FactType> collection = null!;

            When()
                .Query(() => collection, x => x
                    .Match<FactType>(f => f.TestPropertyInt > 0)
                    .Collect()
                    .OrderByDescending(f => f.TestPropertyInt)
                    .ThenBy(f => f.TestPropertyString));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}