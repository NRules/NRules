using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete;

internal class AggregateFactSource(IEnumerable<IFact>? facts) : IFactSource
{
    private static readonly IEnumerable<IFact> Empty = Array.Empty<IFact>();

    public FactSourceType SourceType => FactSourceType.Aggregate;
    public IEnumerable<IFact> Facts { get; } = facts ?? Empty;
}