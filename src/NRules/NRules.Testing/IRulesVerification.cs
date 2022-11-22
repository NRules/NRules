using NRules.Fluent.Dsl;

namespace NRules.Testing;

public interface IRulesVerification
{
    ISelectedRuleVerification Rule();

    ISelectedRuleVerification Rule<T>() where T : Rule;
}
