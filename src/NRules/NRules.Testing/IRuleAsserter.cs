namespace NRules.Testing;

/// <summary>
/// Abstracts assertion logic for the specific testing/assertion framework.
/// </summary>
public interface IRuleAsserter
{
    /// <summary>
    /// Asserts a condition specific to rule firing.
    /// </summary>
    /// <param name="result">Result that need to be verified.</param>
    /// <seealso cref="RuleFireAssertResult"/>
    void Assert(RuleFireAssertResult result);
}
