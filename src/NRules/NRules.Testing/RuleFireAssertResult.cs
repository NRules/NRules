using NRules.Fluent;

namespace NRules.Testing;

/// <summary>
/// Represents assertion result for rule fire condition
/// </summary>
public sealed class RuleFireAssertResult
{
    public RuleFireAssertResult(IRuleMetadata ruleMetadata, int expected, int actual)
    {
        RuleMetadata = ruleMetadata;
        Expected = expected;
        Actual = actual;
    }

    /// <summary>
    /// Gets the rule metatada that condition is associated with
    /// </summary>
    public IRuleMetadata RuleMetadata { get; }

    /// <summary>
    /// Gets the expected number of firings
    /// </summary>
    public int Expected { get; }

    /// <summary>
    /// Gets the actual number of firings
    /// </summary>
    public int Actual { get; }
}
