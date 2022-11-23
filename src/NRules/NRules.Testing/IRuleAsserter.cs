namespace NRules.Testing;

/// <summary>
/// Abstracts assertion logic for specific testing/validation framework
/// </summary>
public interface IRuleAsserter
{
    /// <summary>
    /// Asserts specific rule fire condition. <seealso cref="RuleFireAssertResult"/>
    /// </summary>
    /// <param name="result">Result that need to be verified</param>
    void Assert(RuleFireAssertResult result);
}
