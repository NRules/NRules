using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class FromQueryTest : BaseRuleTestFixture
    {
        [Fact]
        public void From_MultipleKeys_ShouldFireOnceForEachGroup()
        {
            var keys = new[] {"K1", "K2", "K2", "K1"};
            var facts = keys.Select(k => new Fact {Value = k}).ToArray();

            Session.InsertAll(facts);

            Session.Fire();

            AssertFiredTwice();
        }

        [Fact]
        public void From_SingleKey_FiresOnce()
        {
            var keys = new[] { "K1", "K1", "K1", "K1" };
            var facts = keys.Select(k => new Fact { Value = k }).ToArray();

            Session.InsertAll(facts);

            Session.Fire();

            AssertFiredOnce();
        }

        [Fact]
        public void From_NoElements_DoesNotFire()
        {
            Session.InsertAll(new[] {"rubbish", "data"});

            Session.Fire();

            AssertDidNotFire();
        }

        [Fact]
        public void From_HandlesRetracts_FiresThenDoesNotFire()
        {
            var keys = new[] { "K1", "K2", "K2", "K1" };
            var facts = keys.Select(k => new Fact { Value = k }).ToArray();

            Session.InsertAll(facts);

            Session.Fire();

            AssertFiredTwice();

            Session.RetractAll(facts);

            Session.Fire();

            AssertFiredTwice();
        }

        protected override void SetUpRules()
        {
            SetUpRule<FromRule>();
        }
    }

    public class Fact
    {
        public string Value { get; set; }
    }

    public class FromRule : Rule
    {
        public override void Define()
        {
            IEnumerable<Fact> factsAll = null;
            IGrouping<string, Fact> factsGrouped = null;

            When()
                .Query(() => factsAll, q => q
                    .Match<Fact>()
                    .Collect()
                    .Where(c => c.Any()))
                .Query(() => factsGrouped, q => q
                    .From(() => factsAll)
                    .SelectMany(c => c)
                    .GroupBy(f => f.Value));

            Then()
                .Do(ctx => Console.WriteLine($"AllCount={factsAll.Count()}, GroupCount={factsGrouped.Count()}"));
        }
    }
}