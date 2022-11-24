namespace NRules.Testing;

/// <summary>
/// Represents specific rule verification
/// </summary>
public interface IRuleVerification
{
    /// <summary>
    /// Asserts that rule fired exact amount of times using <see cref="IRuleAsserter"/>
    /// </summary>
    /// <param name="expected">Expected number of firings</param>
    void FiredTimes(int expected);
}
