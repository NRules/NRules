using System;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Represents specific rule firing verification.
/// </summary>
public interface IRuleFiringVerification
{
    
    /// <summary>
    /// Asserts that a given rule under test fired with a set of facts matching the specified expectations.
    /// </summary>
    /// <param name="expectations">Expected facts matched by the rule.</param>
    void Fired(params FactConstraint[] expectations);
}

/// <summary>
/// Represents specific rule firing verification.
/// </summary>
public interface IQualifiedRuleFiringVerification : IRuleFiringVerification
{
    /// <summary>
    /// Asserts that a given rule under test fired the given number of times with a set of facts matching the specified expectations.
    /// </summary>
    /// <param name="times">Expected number of rule firings.</param>
    /// <param name="expectations">Expected facts matched by the rule.</param>
    void Fired(Times times, params FactConstraint[] expectations);
}

internal class RuleFiringVerification : IQualifiedRuleFiringVerification
{
    private readonly IRuleDefinition _rule;
    private readonly bool _isExactInvocation;
    private FactConstraint[] _constraints = Array.Empty<FactConstraint>();
    private Times _times = Times.Once;

    public RuleFiringVerification(IRuleDefinition rule, bool isExactInvocation)
    {
        _rule = rule;
        _isExactInvocation = isExactInvocation;
    }

    public void Fired(params FactConstraint[] constraints)
    {
        _constraints = constraints;
    }

    public void Fired(Times times, params FactConstraint[] constraints)
    {
        _times = times;
        _constraints = constraints;
    }

    public IRuleExpectation Build()
    {
        var expectation = new SingleRuleExpectation(_rule, _constraints, _times, _isExactInvocation);
        return expectation;
    }
}