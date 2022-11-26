using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

internal sealed class CachedRuleActivator : IRuleActivator
{
    private readonly IRuleActivator _activator;
    private readonly Dictionary<Type, IEnumerable<Rule>> _rules = new();

    public CachedRuleActivator(IRuleActivator activator) =>
        _activator = activator;

    public IReadOnlyCollection<Type> CachedRuleTypes =>
        _rules.Keys;

    public IEnumerable<Rule> Activate(Type type) =>
        _rules.GetOrAdd(type, ActivateRules);

    private IEnumerable<Rule> ActivateRules(Type t) =>
        _activator.Activate(t).ToArray();
}
