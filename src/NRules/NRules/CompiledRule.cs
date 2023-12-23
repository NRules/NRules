using System.Collections.Generic;
using System.Diagnostics;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules;

internal interface ICompiledRule
{
    int Priority { get; }
    RuleRepeatability Repeatability { get; }
    IRuleDefinition Definition { get; }
    IReadOnlyCollection<Declaration> Declarations { get; }
    IReadOnlyCollection<IRuleAction> Actions { get; }
    IRuleFilter Filter { get; }
    ActionTrigger ActionTriggers { get; }
    IndexMap FactMap { get; }
}

[DebuggerDisplay("{Definition.Name}")]
internal class CompiledRule : ICompiledRule
{
    public CompiledRule(IRuleDefinition definition, IReadOnlyCollection<Declaration> declarations, IReadOnlyCollection<IRuleAction> actions, IRuleFilter filter, IndexMap factMap)
    {
        Definition = definition;
        Filter = filter;
        FactMap = factMap;
        Declarations = declarations;
        Actions = actions;

        foreach (var ruleAction in Actions)
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
    public IReadOnlyCollection<Declaration> Declarations { get; }
    public IReadOnlyCollection<IRuleAction> Actions { get; }
}