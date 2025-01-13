﻿using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneMultiKeySortedCollectionManyChainedThenByRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_FourMatchingFactsAndOneInvalid_FiresOnceWithFourSortedFactsInCollection()
    {
        // Arrange
        var facts = new FactType[]
        {
            new(0, 0, 0, 0, 0),
            new(0, 0, 0, 0, 1),
            new(0, 0, 0, 1, 0),
            new(0, 0, 0, 1, 1),
            new(0, 0, 1, 0, 0),
            new(0, 0, 1, 0, 1),
            new(0, 0, 1, 1, 0),
            new(0, 0, 1, 1, 1),
            new(0, 1, 0, 0, 0),
            new(0, 1, 0, 0, 1),
            new(0, 1, 0, 1, 0),
            new(0, 1, 0, 1, 1),
            new(0, 1, 1, 0, 0),
            new(0, 1, 1, 0, 1),
            new(0, 1, 1, 1, 0),
            new(0, 1, 1, 1, 1),
            new(1, 0, 0, 0, 0),
            new(1, 0, 0, 0, 1),
            new(1, 0, 0, 1, 0),
            new(1, 0, 0, 1, 1),
            new(1, 0, 1, 0, 0),
            new(1, 0, 1, 0, 1),
            new(1, 0, 1, 1, 0),
            new(1, 0, 1, 1, 1),
            new(1, 1, 0, 0, 0),
            new(1, 1, 0, 0, 1),
            new(1, 1, 0, 1, 0),
            new(1, 1, 0, 1, 1),
            new(1, 1, 1, 0, 0),
            new(1, 1, 1, 0, 1),
            new(1, 1, 1, 1, 0),
            new(1, 1, 1, 1, 1)
        };

        Session.InsertAll(facts);

        // Act
        Session.Fire();

        // Assert
        var expectedOrder = facts
            .OrderBy(f => f.TestPropertyInt1)
            .ThenByDescending(f => f.TestPropertyInt2)
            .ThenBy(f => f.TestPropertyInt3)
            .ThenByDescending(f => f.TestPropertyInt4)
            .ThenBy(f => f.TestPropertyInt5)
            .ToArray();

        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType>>()
            .Callback(firedFact =>
            {
                Assert.Equal(32, firedFact.Count());

                for (int i = 0; i < expectedOrder.Length; i++)
                {
                    Assert.Equal(expectedOrder[i], firedFact.ElementAt(i));
                }
            })));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        public FactType(int testInt1, int testInt2, int testInt3, int testInt4, int testInt5)
        {
            TestPropertyInt1 = testInt1;
            TestPropertyInt2 = testInt2;
            TestPropertyInt3 = testInt3;
            TestPropertyInt4 = testInt4;
            TestPropertyInt5 = testInt5;
        }

        public int TestPropertyInt1 { get; set; }
        public int TestPropertyInt2 { get; set; }
        public int TestPropertyInt3 { get; set; }
        public int TestPropertyInt4 { get; set; }
        public int TestPropertyInt5 { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            IEnumerable<FactType> collection = null!;

            When()
                .Query(() => collection, x => x
                    .Match<FactType>()
                    .Collect()
                    .OrderBy(f => f.TestPropertyInt1)
                    .ThenByDescending(f => f.TestPropertyInt2)
                    .ThenBy(f => f.TestPropertyInt3)
                    .ThenByDescending(f => f.TestPropertyInt4)
                    .ThenBy(f => f.TestPropertyInt5));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}