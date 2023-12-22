﻿namespace NRules.Testing;

/// <summary>
/// Abstracts assertion logic for the specific testing/assertion framework.
/// </summary>
public interface IRuleAsserter
{
    /// <summary>
    /// Asserts a condition specific to rule firing.
    /// </summary>
    /// <param name="result">Result that needs to be verified.</param>
    /// <seealso cref="RuleAssertResult"/>
    void Assert(RuleAssertResult result);
}
