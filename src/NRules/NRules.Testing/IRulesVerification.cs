using NRules.Fluent.Dsl;

namespace NRules.Testing;

public interface IRulesVerification
{
    IRuleVerification Rule();

    IRuleVerification Rule<T>() where T : Rule;
}
