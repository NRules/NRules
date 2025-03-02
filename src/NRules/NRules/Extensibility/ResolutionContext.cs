using NRules.RuleModel;

namespace NRules.Extensibility;

/// <summary>
/// Context for dependency resolution.
/// </summary>
public interface IResolutionContext
{
    /// <summary>
    /// Rules engine session that requested dependency resolution.
    /// </summary>
    ISession Session { get; }

    /// <summary>
    /// Rule that requested dependency resolution.
    /// </summary>
    IRuleDefinition Rule { get; }
}

internal class ResolutionContext(ISession session, IRuleDefinition rule) : IResolutionContext
{
    public ISession Session { get; } = session;
    public IRuleDefinition Rule { get; } = rule;
}