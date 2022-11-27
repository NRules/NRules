namespace NRules.Testing;

/// <summary>
/// Represents specific rule firing verification.
/// Failing assertions are raised via the configured <see cref="IRuleAsserter"/>.
/// </summary>
public interface IRuleVerification
{
    /// <summary>
    /// Asserts that a given rule under test fired exactly the specified number of times.
    /// </summary>
    /// <param name="expected">Expected number of firings.</param>
    void FiredTimes(int expected);
}
