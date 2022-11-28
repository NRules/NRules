using System.Collections.Generic;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules;

/// <summary>
/// Action taken on the linked fact.
/// </summary>
public enum LinkedFactAction
{
    /// <summary>
    /// Linked fact is inserted into the session.
    /// </summary>
    Insert,

    /// <summary>
    /// Linked fact is updated in the session.
    /// </summary>
    Update,

    /// <summary>
    /// Linked fact is retracted from the session.
    /// </summary>
    Retract,
}

/// <summary>
/// Collection of linked facts propagated as a set.
/// </summary>
public interface ILinkedFactSet
{
    /// <summary>
    /// Action taken on the linked fact.
    /// </summary>
    LinkedFactAction Action { get; }

    /// <summary>
    /// Linked facts in the set.
    /// </summary>
    IEnumerable<IFact> Facts { get; }

    /// <summary>
    /// Number of linked facts in the set.
    /// </summary>
    int FactCount { get; }
}

internal readonly struct LinkedFactSet : ILinkedFactSet
{
    private readonly List<Fact> _facts = new();

    public LinkedFactSet(LinkedFactAction action)
    {
        Action = action;
    }

    public LinkedFactAction Action { get; }

    IEnumerable<IFact> ILinkedFactSet.Facts => _facts;

    int ILinkedFactSet.FactCount => _facts.Count;

    public IReadOnlyCollection<Fact> Facts => _facts;

    public void AddRange(IEnumerable<Fact> facts)
    {
        _facts.AddRange(facts);
    }
}