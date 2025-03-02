using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Verifies recorded rules invocations against expectations.
/// </summary>
public interface IRulesVerification
{
    /// <summary>
    /// Verifies that the rules under test fired with a set of facts matching the specified expectations.
    /// </summary>
    /// <remarks>Recorded rule invocations are compared to expectations one by one.</remarks>
    /// <param name="action">Expectations configuration action.</param>
    /// <returns>Outcome of validation of recorded rule invocations against the provided expectations.</returns>
    RuleAssertResult VerifySequence(Action<IRuleSequenceFiringVerification> action);

    /// <summary>
    /// Verifies that the rules under test fired with a set of facts matching the specified expectations.
    /// </summary>
    /// <remarks>Recorded rule invocations are compared to expectations according to the expected number of invocations for each rule,
    /// specified using <see cref="Times"/>.</remarks>
    /// <param name="action">Expectations configuration action.</param>
    /// <returns>Outcome of validation of recorded rule invocations against the provided expectations.</returns>
    RuleAssertResult Verify(Action<IRulesFiringVerification> action);
}

internal class RulesVerification(IRulesUnderTest rulesUnderTest, IReadOnlyList<IMatch> invocations)
    : IRulesVerification
{
    public RuleAssertResult VerifySequence(Action<IRuleSequenceFiringVerification> action)
    {
        var verification = new RuleSequenceFiringVerification(rulesUnderTest);
        action(verification);
        var expectation = verification.Build();
        var result = expectation.Verify(invocations);
        return result;
    }

    public RuleAssertResult Verify(Action<IRulesFiringVerification> action)
    {
        var verification = new RulesFiringVerification(rulesUnderTest);
        action(verification);
        var expectation = verification.Build();
        var result = expectation.Verify(invocations);
        return result;
    }
}
