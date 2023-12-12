using System.Linq;

namespace NRules.Testing;

/// <summary>
/// Rules under test compiled into a rules engine session along with the means to verify rules firing.
/// </summary>
public class RulesTestHarness
{
    private readonly RuleInvocationRecorder _invocationRecorder;
    private readonly RulesUnderTest _ruleSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="RulesTestHarness"/> class.
    /// </summary>
    /// <param name="setup">Rules test setup information.</param>
    public RulesTestHarness(IRulesTestSetup setup)
    {
        _ruleSet = new RulesUnderTest(setup.Rules);

        var compiler = new RuleCompiler();
        setup.CompilerSetupAction?.Invoke(compiler);

        var ruleDefinitions = setup.Rules.Select(x => x.Definition);
        var factory = compiler.Compile(ruleDefinitions);
        Session = factory.CreateSession();
        
        _invocationRecorder = new RuleInvocationRecorder(Session);
    }

    /// <summary>
    /// Gets the rules under test.
    /// </summary>
    public IRulesUnderTest RuleSet => _ruleSet;

    /// <summary>
    /// Gets the current rules engine session.
    /// </summary>
    /// <remarks>Lazily created</remarks>
    public ISession Session { get; }

    /// <summary>
    /// Gets the rule invocation recorder to control and inspect rules firing.
    /// </summary>
    public IRuleInvocationRecorder Recorder => _invocationRecorder;

    /// <summary>
    /// Creates a rules verification builder to check rules firing expectations.
    /// </summary>
    /// <returns>Instance of the verification builder.</returns>
    public IRulesVerification GetRulesVerification()
    {
        return new RulesVerification(_ruleSet, Recorder.GetInvocations());
    }
}
