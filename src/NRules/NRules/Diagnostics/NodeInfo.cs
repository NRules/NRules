using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Diagnostics;

internal class NodeInfo
{
    private readonly List<IRuleDefinition> _rules = new();

    public Type OutputType { get; set; }
    public IReadOnlyCollection<IRuleDefinition> Rules => _rules;

    public void Add(IRuleDefinition rule)
    {
        _rules.Add(rule);
    }
}
