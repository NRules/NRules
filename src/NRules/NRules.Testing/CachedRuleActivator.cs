using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Rule activator that decorates existing activator and caches activated instances
/// </summary>
public sealed class CachedRuleActivator : IRuleActivator
{
    private readonly IRuleActivator _activator;
    private readonly Dictionary<Type, IEnumerable<Rule>> _rules = new();

    public CachedRuleActivator(IRuleActivator activator) =>
        _activator = activator;

    /// <summary>
    /// Exposes already cached types for monitoring/asserting purposes
    /// </summary>
    public IReadOnlyCollection<Type> CachedRuleTypes =>
        _rules.Keys;

    /// <inheritdoc cref="IRuleActivator.Activate(Type)"/>
    public IEnumerable<Rule> Activate(Type type) =>
        _rules.GetOrAdd(type, ActivateRules);

    private IEnumerable<Rule> ActivateRules(Type t) =>
        _activator.Activate(t).ToArray();
}
