namespace NRules.Testing;

/// <summary>
/// Represents specific rule verification
/// </summary>
public interface IRuleVerification
{
    /// <summary>
    /// Asserts that rule was fired exact amount of times
    /// </summary>
    /// <param name="expected">Expected number of firings</param>
    void FiredTimes(int expected);
}
