﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Fluent;

/// <summary>
/// Fluent specification to load rule definitions via reflection.
/// </summary>
public interface IRuleLoadSpec
{
    /// <summary>
    /// Enables/disables discovery of private rule classes.
    /// Default is off.
    /// </summary>
    /// <param name="include">Include private types if <c>true</c>, don't include otherwise.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec PrivateTypes(bool include = true);

    /// <summary>
    /// Enables/disables discovery of nested rule classes.
    /// Default is off.
    /// </summary>
    /// <param name="include">Include nested types if <c>true</c>, don't include otherwise.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec NestedTypes(bool include = true);

    /// <summary>
    /// Specifies to load all rule definitions from a given collection of assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to load from.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec From(params Assembly[] assemblies);
    
    /// <summary>
    /// Specifies to load all rule definitions from a given collection of assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to load from.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec From(IEnumerable<Assembly> assemblies);

    /// <summary>
    /// Specifies to load rule definitions from a given collection of types.
    /// </summary>
    /// <param name="types">Types that represent rule definitions.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec From(params Type[] types);

    /// <summary>
    /// Specifies to load rule definitions from a given collection of types.
    /// </summary>
    /// <param name="types">Types that represent rule definitions.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec From(IEnumerable<Type> types);

    /// <summary>
    /// Specifies to load rule definitions by scanning types/assemblies.
    /// </summary>
    /// <param name="scanAction">Assembly/type scan action.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec From(Action<IRuleTypeScanner> scanAction);

    /// <summary>
    /// Specifies which rules to load by filtering on rule's metadata.
    /// </summary>
    /// <param name="filter">Filter condition based on rule's metadata.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec Where(Func<IRuleMetadata, bool> filter);

    /// <summary>
    /// Specifies the name of the rule set where the rules are loaded to.
    /// If not provided, loads rules into default rule set.
    /// </summary>
    /// <param name="ruleSetName">Name of the rule set to load rules to.</param>
    /// <returns>Spec to continue fluent configuration.</returns>
    IRuleLoadSpec To(string ruleSetName);
}

internal class RuleLoadSpec : IRuleLoadSpec
{
    private readonly IRuleActivator _activator;
    private readonly RuleTypeScanner _typeScanner = new();
    private Func<IRuleMetadata, bool>? _filter;

    public RuleLoadSpec(IRuleActivator activator)
    {
        _activator = activator;
    }

    public string? RuleSetName { get; private set; }

    public IRuleLoadSpec PrivateTypes(bool include = true)
    {
        _typeScanner.PrivateTypes(include);
        return this;
    }

    public IRuleLoadSpec NestedTypes(bool include = true)
    {
        _typeScanner.NestedTypes(include);
        return this;
    }

    public IRuleLoadSpec From(params Assembly[] assemblies)
    {
        _typeScanner.Assembly(assemblies);
        return this;
    }

    public IRuleLoadSpec From(IEnumerable<Assembly> assemblies)
    {
        _typeScanner.Assembly(assemblies.ToArray());
        return this;
    }

    public IRuleLoadSpec From(params Type[] types)
    {
        _typeScanner.Type(types);
        return this;
    }

    public IRuleLoadSpec From(IEnumerable<Type> types)
    {
        _typeScanner.Type(types.ToArray());
        return this;
    }

    public IRuleLoadSpec From(Action<IRuleTypeScanner> scanAction)
    {
        scanAction(_typeScanner);
        return this;
    }

    public IRuleLoadSpec Where(Func<IRuleMetadata, bool> filter)
    {
        if (_filter != null)
            throw new InvalidOperationException("Rule load specification can only have a single 'Where' clause");
        
        _filter = filter;
        return this;
    }

    public IRuleLoadSpec To(string ruleSetName)
    {
        if (RuleSetName != null)
            throw new InvalidOperationException("Rule load specification can only have a single 'To' clause");
        
        RuleSetName = ruleSetName;
        return this;
    }

    public IEnumerable<IRuleDefinition> Load()
    {
        var rules = GetRuleTypes()
            .SelectMany(Activate);
        var factory = new RuleDefinitionFactory();
        var ruleDefinitions = factory.Create(rules);
        return ruleDefinitions;
    }

    private IEnumerable<Type> GetRuleTypes()
    {
        if (_filter != null)
        {
            return _typeScanner.GetRuleTypes()
                .Select(ruleType => new RuleMetadata(ruleType))
                .Where(x => _filter(x))
                .Select(x => x.RuleType);
        }
        return _typeScanner.GetRuleTypes();
    }

    private IReadOnlyCollection<Rule> Activate(Type type)
    {
        try
        {
            var ruleInstances = _activator.Activate(type);
            return ruleInstances.ToList();
        }
        catch (Exception e)
        {
            throw new RuleActivationException("Failed to activate rule type", type, e);
        }
    }
}
