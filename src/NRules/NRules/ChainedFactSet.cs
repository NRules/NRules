using System.Collections.Generic;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules;

/// <summary>
/// Action taken on the chained fact.
/// </summary>
public enum ChainedFactAction
{
    /// <summary>
    /// Chained fact is inserted into the session.
    /// </summary>
    Insert,

    /// <summary>
    /// Chained fact is updated in the session.
    /// </summary>
    Update,

    /// <summary>
    /// Chained fact is retracted from the session.
    /// </summary>
    Retract,
}

/// <summary>
/// Collection of chained facts propagated as a set.
/// </summary>
public interface IChainedFactSet
{
    /// <summary>
    /// Action taken on the chained fact.
    /// </summary>
    ChainedFactAction Action { get; }

    /// <summary>
    /// Chained facts in the set.
    /// </summary>
    IEnumerable<IFact> Facts { get; }

    /// <summary>
    /// Number of chained facts in the set.
    /// </summary>
    int FactCount { get; }
}

internal struct ChainedFactSet : IChainedFactSet
{
    public ChainedFactAction Action { get; }
    IEnumerable<IFact> IChainedFactSet.Facts => Facts;
    int IChainedFactSet.FactCount => Facts.Count;

    public List<Fact> Facts { get; }

    public ChainedFactSet(ChainedFactAction action)
    {
        Action = action;
        Facts = new List<Fact>();
    }
}