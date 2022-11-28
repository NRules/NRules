using System;
using System.Collections.Generic;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Diagnostics;

internal class NodeInfo : ICanDeepClone<NodeInfo>
{
    private readonly List<IRuleDefinition> _rules = new();

    public Type OutputType { get; set; }
    public IEnumerable<IRuleDefinition> Rules => _rules;

    public void Add(IRuleDefinition rule)
    {
        _rules.Add(rule);
    }

    public NodeInfo DeepClone()
    {
        var info = new NodeInfo();
        CloneInto(info);
        return info;
    }

    public void CloneInto(NodeInfo info)
    {
        info.OutputType = OutputType;
        _rules.CloneInto(info._rules);
    }
}
