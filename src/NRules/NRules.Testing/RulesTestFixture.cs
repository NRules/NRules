using System;

namespace NRules.Testing;

/// <summary>
/// Fixture to test rules.
/// </summary>
public class RulesTestFixture
{
    private readonly Lazy<RulesTestHarness> _testHarness;

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
    public IRuleAsserter Asserter { get; set; }

    /// <summary>
    /// Gets the test setup to register rules under test.
    /// </summary>
    public IRulesTestSetup Setup { get; }

    /// <summary>
    /// Gets the current rules engine session.
    /// </summary>
    /// <remarks>Lazily created.</remarks>
    public ISession Session => _testHarness.Value.Session;

    /// <summary>
    /// Gets the rule invocation recorder to inspect rule firing.
    /// </summary>
    /// <remarks>Lazily created.</remarks>
    public IRuleInvocationRecorder Recorder => _testHarness.Value.Recorder;

    /// <summary>
    /// Configures rule firing assertions using an expectation builder.
    /// </summary>
    /// <remarks>Lazily created.</remarks>
    public void Verify(Action<IRuleVerification> buildAction)
    {
        var verification = _testHarness.Value.GetRulesVerification();
        var result = verification.Verify(buildAction);
        Asserter.Assert(result);
    }

    /// <summary>
    /// Configures exact rule sequence firing assertions using an expectation builder.
    /// </summary>
    /// <remarks>Lazily created.</remarks>
    public void VerifySequence(Action<IRuleSequenceVerification> buildAction)
    {
        var verification = _testHarness.Value.GetRulesVerification();
        var result = verification.VerifySequence(buildAction);
        Asserter.Assert(result);
    }
}
