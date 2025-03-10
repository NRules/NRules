﻿using System;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Represents specific rule firing verification.
/// </summary>
public interface IRuleFiringVerification
{
    /// <summary>
    /// Asserts that a given rule under test fired with a set of facts matching the specified constraints.
    /// </summary>
    /// <param name="constraints">Constraints narrowing down the rule firing expectation.</param>
    void Fired(params FactConstraint[] constraints);
}

/// <summary>
/// Represents specific rule firing verification that uses qualified rule firing expectations.
/// </summary>
public interface IQualifiedRuleFiringVerification : IRuleFiringVerification
{
    /// <summary>
    /// Asserts that a given rule under test fired the given number of times with a set of facts matching the specified constraints.
    /// </summary>
    /// <param name="times">Expected number of rule firings.</param>
    /// <param name="constraints">Constraints narrowing down the rule firing expectation.</param>
    void Fired(Times times, params FactConstraint[] constraints);
}

internal class RuleFiringVerification(IRuleDefinition rule, bool isExact) : IQualifiedRuleFiringVerification
{
    private FactConstraint[] _constraints = [];
    private Times _times = Times.Once;

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
        var expectation = new SingleRuleExpectation(rule, _constraints, _times, isExact);
        return expectation;
    }
}