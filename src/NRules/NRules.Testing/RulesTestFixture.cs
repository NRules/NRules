﻿using System;

namespace NRules.Testing;

/// <summary>
/// Fixture to test rules.
/// </summary>
public class RulesTestFixture
{
    private readonly Lazy<RulesTestHarness> _testHarness;
    private IRuleAsserter? _asserter;

    /// <summary>
    /// Initializes a new instance of the <see cref="RulesTestFixture"/> class.
    /// </summary>
    public RulesTestFixture()
    {
        Setup = new RulesTestSetup();
        _testHarness = new Lazy<RulesTestHarness>(() => new RulesTestHarness(Setup));
    }

    /// <summary>
    /// Gets or sets the framework-specific asserter to validate rule firing expectations.
    /// </summary>
    /// <remarks>If asserter is not set, it's initialized to a <see cref="RuleAsserter"/> on first access.</remarks>
    public IRuleAsserter Asserter
    {
        get => _asserter ??= new RuleAsserter();
        set => _asserter = value;
    }

    /// <summary>
    /// Gets the test setup to register rules under test.
    /// </summary>
    public IRulesTestSetup Setup { get; }

    /// <summary>
    /// Gets the current rules engine session.
    /// </summary>
    public ISession Session => _testHarness.Value.Session;

    /// <summary>
    /// Gets the rule invocation recorder to inspect rule firing.
    /// </summary>
    public IRuleInvocationRecorder Recorder => _testHarness.Value.Recorder;

    /// <summary>
    /// Configures assertions for rules firing using an expectation builder.
    /// </summary>
    public void Verify(Action<IRulesFiringVerification> buildAction)
    {
        var verification = _testHarness.Value.GetRulesVerification();
        var result = verification.Verify(buildAction);
        Asserter.Assert(result);
    }

    /// <summary>
    /// Configures assertions of the exact rules firing sequence using an expectation builder.
    /// </summary>
    public void VerifySequence(Action<IRuleSequenceFiringVerification> buildAction)
    {
        var verification = _testHarness.Value.GetRulesVerification();
        var result = verification.VerifySequence(buildAction);
        Asserter.Assert(result);
    }
}
