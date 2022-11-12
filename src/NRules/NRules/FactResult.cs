namespace NRules;

/// <summary>
/// Result of an operation on a set of facts.
/// </summary>
public interface IFactResult
{
    /// <summary>
    /// Facts on which the operation failed.
    /// </summary>
    IReadOnlyCollection<object> Failed { get; }
}

internal class FactResult : IFactResult
{
    private readonly IReadOnlyCollection<object> _failed;

    internal FactResult(IReadOnlyCollection<object> failed) => _failed = failed;

    public IReadOnlyCollection<object> Failed => _failed;
}