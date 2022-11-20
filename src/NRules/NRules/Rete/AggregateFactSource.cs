using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete;

internal class AggregateFactSource : IFactSource
{
    private static readonly IEnumerable<IFact> Empty = Array.Empty<IFact>();

    public AggregateFactSource(IEnumerable<IFact> facts)
    {
        Facts = facts ?? Empty;
    }

    public FactSourceType SourceType => FactSourceType.Aggregate;
    public IEnumerable<IFact> Facts { get; }
}