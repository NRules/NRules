namespace NRules.Testing;

/// <summary>
/// Fluent builder for specifying the number of rule invocations in a rule firing expectation.
/// </summary>
public struct Times
{
    /// <summary>
    /// Rule should never fire.
    /// </summary>
    public static Times Never => new(0);

    /// <summary>
    /// Rule should fire exactly once.
    /// </summary>
    public static Times Once => new(1);

    /// <summary>
    /// Rule should fire exactly twice.
    /// </summary>
    public static Times Twice => new(2);

    /// <summary>
    /// Rule should fire exactly the specified number of times.
    /// </summary>
    /// <param name="value">Number of times the rule is expected to fire.</param>
    public static Times Exactly(int value) => new(value);

    private Times(int value)
    {
        Value = value;
    }

    internal int Value { get; }
}