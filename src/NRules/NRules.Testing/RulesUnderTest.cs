using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Testing;

/// <summary>
/// Represents the rules under test.
/// </summary>
public interface IRulesUnderTest
{
    /// <summary>
    /// Gets the rules under test.
    /// </summary>
    IReadOnlyCollection<RuleInfo> Rules { get; }

    /// <summary>
    /// Gets the rule information for the specified rule type.
    /// </summary>
    /// <param name="ruleType">Type of the rule to look for.</param>
    /// <returns>Found rule.</returns>
    RuleInfo GetRuleInfo(Type ruleType);

    /// <summary>
    /// Gets the rule information for the single rule under test.
    /// </summary>
    /// <returns>Found rule.</returns>
    RuleInfo GetSingle();
}

internal class RulesUnderTest : IRulesUnderTest
{
    private readonly IReadOnlyCollection<RuleInfo> _rules;
    private readonly Dictionary<Type, RuleInfo> _ruleMap;

    public RulesUnderTest(IReadOnlyCollection<RuleInfo> rules)
    {
        _rules = rules;
        _ruleMap = rules.ToDictionary(x => x.Type, x => x);
    }

    public IReadOnlyCollection<RuleInfo> Rules => _rules;

    public RuleInfo GetRuleInfo(Type ruleType)
    {
        if (_ruleMap.TryGetValue(ruleType, out var ruleInfo))
            return ruleInfo;
     
        throw new ArgumentException($"Rule type {ruleType} is not registered");
    }

    public RuleInfo GetSingle()
    {
        return _rules.Count switch
        {
            0 => throw new ArgumentException("Expected single rule test, but found no rules registered"),
            1 => _rules.Single(),
            _ => throw new ArgumentException("Expected single rule test, but found multiple rules registered"),
        };
    }
}
