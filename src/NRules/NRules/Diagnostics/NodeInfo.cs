using System;
using System.Collections;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Diagnostics;

internal class NodeInfo : IEnumerable<IRuleDefinition>
{
    private readonly List<IRuleDefinition> _rules;

    public NodeInfo(Type? outputType = null, params IRuleDefinition[] rules)
    {
        OutputType = outputType;
        _rules = new(rules);
    }

    public Type? OutputType { get; }

    public void Add(IRuleDefinition rule)
    {
        _rules.Add(rule);
    }

    public IEnumerator<IRuleDefinition> GetEnumerator()
    {
        return _rules.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_rules).GetEnumerator();
    }
}
