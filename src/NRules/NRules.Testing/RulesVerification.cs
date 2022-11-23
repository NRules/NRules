using System;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

internal sealed class RulesVerification : IRulesVerification
{
    private readonly IRuleAsserter _asserter;
    private readonly IRuleAccessor _accessor;

    public RulesVerification(IRuleAsserter asserter, IRuleAccessor accessor)
    {
        _asserter = asserter;
        _accessor = accessor;
    }

    public IRuleVerification Rule() =>
        _accessor.RegisteredRuleTypes.Count switch
        {
            0 => throw new ArgumentException("Expected single rule test, but found no rules registered"),
            1 => Rule(_accessor.RegisteredRuleTypes.First()),
            _ => throw new ArgumentException("Expected single rule test, but found multiple rules registered"),
        };

    public IRuleVerification Rule<T>()
        where T : Rule =>
        Rule(typeof(T));

    public IRuleVerification Rule(Type ruleType)
    {
        var ruleMetadata = _accessor.GetRule(ruleType);
        return Rule(ruleMetadata);
    }

    private IRuleVerification Rule(IRuleMetadata ruleMetadata)
    {
        var matches = _accessor.GetFiredRuleMatches(ruleMetadata.Name);
        return new RuleVerification(_asserter, ruleMetadata, matches);
    }
}
