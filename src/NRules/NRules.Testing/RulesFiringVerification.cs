using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Fluent interface to build rules firing verification.
/// </summary>
public interface IRulesFiringVerification<out TVerification>
{
    /// <summary>
    /// Firing verification for a single registered rule under test.
    /// </summary>
    /// <returns>Specific rule firing verification builder.</returns>
    TVerification Rule();

    /// <summary>
    /// Firing verification for a registered rule under test with the specified rule type.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <typeparam name="TRule">Type of the rule to verify its firing.</typeparam>
    /// <returns>Specific rule firing verification builder.</returns>
    TVerification Rule<TRule>() where TRule : Rule;

    /// <summary>
    /// Firing verification for a registered rule under test with the specified rule type.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <param name="ruleType"><see cref="Type"/> of the rule to verify its firing.</param>
    /// <returns>Specific rule firing verification builder.</returns>
    TVerification Rule(Type ruleType);
    
    /// <summary>
    /// Firing verification for a registered rule under test with the specified name.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <param name="ruleName">Name of the rule to verify its firing.</param>
    /// <returns>Specific rule firing verification builder.</returns>
    TVerification Rule(string ruleName);
}

/// <summary>
/// Fluent interface to build specific rule firing verification.
/// </summary>
public interface IRulesFiringVerification : IRulesFiringVerification<IQualifiedRuleFiringVerification>
{
}

/// <summary>
/// Fluent interface to build exact rule sequence firing verification.
/// </summary>
public interface IRuleSequenceFiringVerification : IRulesFiringVerification<IRuleFiringVerification>
{
}

internal abstract class RulesFiringVerification<TVerification> : IRulesFiringVerification<TVerification>
{
    private readonly IRulesUnderTest _rulesUnderTest;
    private readonly List<RuleFiringVerification> _verifications = new();

    protected RulesFiringVerification(IRulesUnderTest rulesUnderTest)
    {
        _rulesUnderTest = rulesUnderTest;
    }

    public TVerification Rule()
    {
        return Rule(_rulesUnderTest.GetSingle());
    }

    public TVerification Rule<TRule>() where TRule : Rule
    {
        return Rule(typeof(TRule));
    }

    public TVerification Rule(Type ruleType)
    {
        return Rule(_rulesUnderTest.GetRuleInfo(ruleType));
    }

    public TVerification Rule(string ruleName)
    {
        return Rule(_rulesUnderTest.GetRuleInfo(ruleName));
    }

    protected void AddVerification(RuleFiringVerification verification)
    {
        _verifications.Add(verification);
    }

    protected abstract TVerification Rule(RuleInfo rule);

    public IRuleExpectation Build()
    {
        var expectations = _verifications.Select(x => x.Build()).ToArray();
        return new MultiRuleExpectation(expectations);
    }
}

internal class RuleSequenceFiringVerification : RulesFiringVerification<IRuleFiringVerification>, IRuleSequenceFiringVerification
{

    public RuleSequenceFiringVerification(IRulesUnderTest rulesUnderTest) : base(rulesUnderTest)
    {
    }

    protected override IRuleFiringVerification Rule(RuleInfo rule)
    {
        var verification = new RuleFiringVerification(rule.Definition, isExact: true);
        AddVerification(verification);
        return verification;
    }
}

internal class RulesFiringVerification : RulesFiringVerification<IQualifiedRuleFiringVerification>, IRulesFiringVerification
{
    public RulesFiringVerification(IRulesUnderTest rulesUnderTest) : base(rulesUnderTest)
    {
    }

    protected override IQualifiedRuleFiringVerification Rule(RuleInfo rule)
    {
        var verification = new RuleFiringVerification(rule.Definition, isExact: false);
        AddVerification(verification);
        return verification;
    }
}