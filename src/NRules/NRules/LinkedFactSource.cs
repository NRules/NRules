using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules;

/// <summary>
/// Fact source for linked facts.
/// </summary>
public interface ILinkedFactSource : IFactSource
{
    /// <summary>
    /// Rule that generated the linked fact.
    /// </summary>
    IRuleDefinition Rule { get; }
}

internal class LinkedFactSource(Activation activation) : ILinkedFactSource
{
    public FactSourceType SourceType => FactSourceType.Linked;
    public IEnumerable<IFact> Facts => activation.Facts;
    public IRuleDefinition Rule => activation.Rule;
}
