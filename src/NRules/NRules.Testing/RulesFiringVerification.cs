using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Fluent interface to build rules verification.
/// </summary>
public interface IRuleVerification<out TVerification>
{
    /// <summary>
    /// Single registered rule under test.
    /// </summary>
    /// <returns>Specific rule verification builder.</returns>
    TVerification Rule();

    /// <summary>
    /// Registered rule under test with the specifies rule type.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <typeparam name="TRule">Type of the rule to look for.</typeparam>
    /// <returns>Specific rule verification builder.</returns>
    TVerification Rule<TRule>() where TRule : Rule;

    /// <summary>
    /// Registered rule under test with the specifies rule type.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <param name="ruleType"><see cref="Type"/> of the rule to look for.</param>
    /// <returns>Specific rule verification builder.</returns>
    TVerification Rule(Type ruleType);
}

/// <summary>
/// Fluent interface to build specific rule invocation verification.
/// </summary>
public interface IRuleVerification : IRuleVerification<IQualifiedRuleFiringVerification>
{
}

/// <summary>
/// Fluent interface to build specific rule invocation verification.
/// </summary>
public interface IRuleSequenceVerification : IRuleVerification<IRuleFiringVerification>
{
}

internal abstract class RulesFiringVerification<TVerification> : IRuleVerification<TVerification>
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

internal class RulesSequenceFiringVerification : RulesFiringVerification<IRuleFiringVerification>, IRuleSequenceVerification
{

    public RulesSequenceFiringVerification(IRulesUnderTest rulesUnderTest) : base(rulesUnderTest)
    {
    }

    protected override IRuleFiringVerification Rule(RuleInfo rule)
    {
        var verification = new RuleFiringVerification(rule.Definition, isExactInvocation: true);
        AddVerification(verification);
        return verification;
    }
}

internal class RulesFiringVerification : RulesFiringVerification<IQualifiedRuleFiringVerification>, IRuleVerification
{
    public RulesFiringVerification(IRulesUnderTest rulesUnderTest) : base(rulesUnderTest)
    {
    }

    protected override IQualifiedRuleFiringVerification Rule(RuleInfo rule)
    {
        var verification = new RuleFiringVerification(rule.Definition, isExactInvocation: false);
        AddVerification(verification);
        return verification;
    }
}