using System.Text;

namespace NRules.Testing;

/// <summary>
/// Represents the result for a rule firing assertion.
/// </summary>
public sealed class RuleAssertResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleAssertResult"/> class.
    /// </summary>
    /// <param name="ruleName">Name of the rule corresponding to the assertion.</param>
    /// <param name="status">Assertion outcome.</param>
    /// <param name="assertionText">Text that describes the assertion.</param>
    /// <param name="expected">Expected outcome.</param>
    /// <param name="actual">Actual outcome.</param>
    public RuleAssertResult(string? ruleName, RuleAssertStatus status, string assertionText, object expected, object actual)
    {
        RuleName = ruleName;
        Status = status;
        AssertionText = assertionText;
        Expected = expected;
        Actual = actual;
    }

    /// <summary>
    /// Gets the name of the rule corresponding to the assertion.
    /// </summary>
    public string? RuleName { get; }

    /// <summary>
    /// Gets the rule assertion outcome.
    /// </summary>
    public RuleAssertStatus Status { get; }

    /// <summary>
    /// Gets the text that describes the assertion.
    /// </summary>
    public string AssertionText { get; }

    /// <summary>
    /// Gets the expected assertion outcome.
    /// </summary>
    public object Expected { get; }

    /// <summary>
    /// Gets the actual assertion outcome.
    /// </summary>
    public object Actual { get; }

    /// <summary>
    /// Gets the message that describes the assertion outcome.
    /// </summary>
    public string GetMessage()
    {
        var messageBuilder = new StringBuilder();
        if (RuleName != null)
        {
            messageBuilder.Append($"Rule '{RuleName}': ");
        }
        messageBuilder.Append(AssertionText);
        messageBuilder.Append($"; Expected: {Expected}");
        messageBuilder.Append($"; Actual: {Actual}");
        return messageBuilder.ToString();
    }
}
