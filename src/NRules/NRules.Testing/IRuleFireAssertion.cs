using NRules.Fluent.Dsl;

namespace NRules.Testing;

public interface IRuleFireAssertion
{
    IRuleFireAssertResult IsFiredTimes(int expected);

    IRuleFireAssertResult IsFiredTimes<T>(int expected) where T : Rule;
}
