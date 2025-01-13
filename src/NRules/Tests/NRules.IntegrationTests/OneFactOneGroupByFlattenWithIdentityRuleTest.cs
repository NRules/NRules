using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class OneFactOneGroupByFlattenWithIdentityRuleTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_UpdatesWithSameIdButDifferentCount_FiresWithNewCount2()
    {
        //Arrange
        var factsToInsert = new object[]
        {
            new FactType {Id = 1, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group"},
            new FactType {Id = 2, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
            new FactType {Id = 3, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group3"},
            new FactType {Id = 4, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group2"}
        };
        var factsToUpdate = new object[]
        {
            new FactType {Id = 1, TestCount = 2, GroupingProperty = "GP1", GroupingProperty2 = "Group"},
            new FactType {Id = 2, TestCount = 3, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
            new FactType {Id = 3, TestCount = 1, GroupingProperty = "GP2", GroupingProperty2 = "Group"},
            new FactType {Id = 4, TestCount = 1, GroupingProperty = "GP1", GroupingProperty2 = "Group"}
        };
        Session.InsertAll(factsToInsert);
        Session.UpdateAll(factsToUpdate);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IGrouping<string, FactType>>()
            .Callback(firedFacts =>
            {
                Assert.Equal(4, firedFacts.Count());
                Assert.Equal(1, firedFacts.Count(x => x.TestCount == 3));
                Assert.Equal(1, firedFacts.Count(x => x.TestCount == 2));
                Assert.Equal(2, firedFacts.Count(x => x.TestCount == 1));
            })));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType : IIdentityProvider
    {
        public long Id { get; set; }
        public int TestCount { get; set; }
        [NotNull]
        public string? GroupingProperty { get; set; }
        [NotNull]
        public string? GroupingProperty2 { get; set; }

        public object GetIdentity() => Id;
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            IEnumerable<FactType> facts = null!;

            When()
                .Query(() => facts, q => q
                    .Match<FactType>(f => f.Id != 0)
                    .GroupBy(f => f.GroupingProperty)
                    .Where(x => x.Select(xx => xx.Id).Distinct().Count() > 1)
                    .SelectMany(x => x)
                    .GroupBy(x => x.GroupingProperty2));
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}
