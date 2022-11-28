using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules;

internal interface ICompiledRule
{
    int Priority { get; }
    RuleRepeatability Repeatability { get; }
    IRuleDefinition Definition { get; }
    IEnumerable<Declaration> Declarations { get; }
    IEnumerable<IRuleAction> Actions { get; }
    IRuleFilter Filter { get; }
    ActionTrigger ActionTriggers { get; }
    IndexMap FactMap { get; }
}

[DebuggerDisplay("{Definition.Name}")]
internal class CompiledRule : ICompiledRule
{
    private readonly IEnumerable<Declaration> _declarations;
    private readonly IEnumerable<IRuleAction> _actions;

    public CompiledRule(IRuleDefinition definition, IEnumerable<Declaration> declarations, IReadOnlyCollection<IRuleAction> actions, IRuleFilter filter, IndexMap factMap)
    {
        Definition = definition;
        Filter = filter;
        FactMap = factMap;
        _declarations = declarations.ToList();
        _actions = actions;

        foreach (var ruleAction in _actions)
        {
            ActionTriggers |= ruleAction.Trigger;
        }
    }

    public int Priority => Definition.Priority;
    public RuleRepeatability Repeatability => Definition.Repeatability;
    public IRuleDefinition Definition { get; }
    public IRuleFilter Filter { get; }
    public ActionTrigger ActionTriggers { get; }
    public IndexMap FactMap { get; }

    public IEnumerable<Declaration> Declarations => _declarations;
    public IEnumerable<IRuleAction> Actions => _actions;
}