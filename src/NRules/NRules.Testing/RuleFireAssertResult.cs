using NRules.Fluent;

namespace NRules.Testing;

/// <summary>
/// Represents assertion result for a rule firing condition.
/// </summary>
public sealed class RuleFireAssertResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleFireAssertResult"/> class.
    /// </summary>
    /// <param name="ruleMetadata">Rule metadata corresponding to the rule firing that the assertion is associated with.</param>
    /// <param name="expected">Expected number of rule firings.</param>
    /// <param name="actual">Actual number of rule firings.</param>
    public RuleFireAssertResult(IRuleMetadata ruleMetadata, int expected, int actual)
    {
        RuleMetadata = ruleMetadata;
        Expected = expected;
        Actual = actual;
    }

    /// <summary>
    /// Gets the rule metatada corresponding to the rule firing that the assertion is associated with.
    /// </summary>
    public IRuleMetadata RuleMetadata { get; }

    /// <summary>
    /// Gets the expected number of rule firings.
    /// </summary>
    public int Expected { get; }

    /// <summary>
    /// Gets the actual number of rule firings.
    /// </summary>
    public int Actual { get; }
}
