using System.Collections.Generic;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

public interface IRulesTestFixture
{
    ISession Session { get; }

    IRepositorySetup Setup { get; }

    IRulesVerification Verify { get; }

    T GetFiredFact<T>();

    T GetFiredFact<T>(int index);

    IEnumerable<T> GetFiredFacts<T>();

    T GetRuleInstance<T>() where T : Rule;
}
