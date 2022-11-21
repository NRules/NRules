using NRules.Fluent.Dsl;
using NRules.Testing;

namespace NRules.IntegrationTests.TestAssets;

public static class RuleTestFixtureExtensions
{
    public static void AssertFiredOnce(this RuleTestFixture fixture) =>
        fixture.IsFiredTimes(1).Assert();

    public static void AssertFiredTwice(this RuleTestFixture fixture) =>
        fixture.IsFiredTimes(2).Assert();

    public static void AssertDidNotFire(this RuleTestFixture fixture) =>
        fixture.IsFiredTimes(0).Assert();

    public static void AssertFiredTimes(this RuleTestFixture fixture, int expected) =>
        fixture.IsFiredTimes(expected).Assert();

    public static void AssertFiredOnce<T>(this RuleTestFixture fixture)
        where T : Rule =>
        fixture.IsFiredTimes<T>(1).Assert();

    public static void AssertFiredTwice<T>(this RuleTestFixture fixture)
        where T : Rule =>
        fixture.IsFiredTimes<T>(2).Assert();

    public static void AssertDidNotFire<T>(this RuleTestFixture fixture)
        where T : Rule =>
        fixture.IsFiredTimes<T>(0).Assert();

    public static void AssertFiredTimes<T>(this RuleTestFixture fixture, int expected)
        where T : Rule =>
        fixture.IsFiredTimes<T>(expected).Assert();

    private static void Assert(this IRuleFireAssertResult result)
    {
        if (result.Expected != result.Actual)
        {
            throw new RuleFiredAssertionException(result.Expected, result.Actual, result.Rule.Name);
        }
    }
}
