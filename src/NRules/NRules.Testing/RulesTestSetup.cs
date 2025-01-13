using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Sets up rules test.
/// </summary>
public interface IRulesTestSetup
{
    /// <summary>
    /// Rules under test.
    /// </summary>
    public IReadOnlyCollection<RuleInfo> Rules { get; }

    /// <summary>
    /// Gets or sets the action to apply when creating the <see cref="RuleCompiler"/>.
    /// </summary>
    public Action<RuleCompiler> CompilerSetupAction { get; set; }

    /// <summary>
    /// Adds specific rule under test to the setup.
    /// </summary>
    /// <typeparam name="T">Type of the rule to add.</typeparam>
    /// <remarks>
    /// If <typeparamref name="T"/> is not concrete, it will be ignored.
    /// </remarks>
    void Rule<T>() where T : Rule;

    /// <summary>
    /// Adds specific rule under test to the setup.
    /// </summary>
    /// <param name="ruleType"><see cref="Type"/> of the rule to add.</param>
    /// <remarks>
    /// If <paramref name="ruleType"/> is not derived from <see cref="Fluent.Dsl.Rule"/> or is not concrete,
    /// it will be ignored.
    /// </remarks>
    void Rule(Type ruleType);

    /// <summary>
    /// Adds specific rule under test to the setup.
    /// </summary>
    /// <param name="ruleInstance">Rule instance to add.</param>
    void Rule(Rule ruleInstance);
    
    /// <summary>
    /// Adds specific rule under test to the setup.
    /// </summary>
    /// <param name="ruleDefinition">Rule definition to add.</param>
    /// <remarks>
    /// Rules registered as <see cref="IRuleDefinition"/> cannot participate in assertions
    /// that use Fluent DSL information, such as the rule CLR type.
    /// </remarks>
    void Rule(IRuleDefinition ruleDefinition);
}

internal sealed class RulesTestSetup : IRulesTestSetup
{
    private readonly IRuleActivator _ruleActivator = new DefaultRuleActivator();
    private readonly RuleDefinitionFactory _ruleDefinitionFactory = new();
    private readonly List<RuleInfo> _rules = new();

    public IReadOnlyCollection<RuleInfo> Rules => _rules;
    public Action<RuleCompiler> CompilerSetupAction { get; set; } = _ => { };

    public void Rule<T>() where T : Rule => Rule(typeof(T));

    public void Rule(Type ruleType)
    {
        var rules = _ruleActivator.Activate(ruleType).ToArray();
        foreach (var rule in rules)
        {
            Rule(rule);
        }
    }

    public void Rule(Rule ruleInstance)
    {
        var definition = _ruleDefinitionFactory.Create(ruleInstance);
        var ruleInfo = new RuleInfo(ruleInstance.GetType(), ruleInstance, definition);
        AddRule(ruleInfo);
    }
    
    public void Rule(IRuleDefinition ruleDefinition)
    {
        var ruleInfo = new RuleInfo(ruleDefinition);
        AddRule(ruleInfo);
    }
    
    private void AddRule(RuleInfo ruleInfo)
    {
        if (ruleInfo.Type != null && _rules.Any(x => x.Type == ruleInfo.Type))
            throw new ArgumentException($"Rule with type {ruleInfo.Type} is already registered");
        if (_rules.Any(x => string.Equals(x.Definition.Name, ruleInfo.Definition.Name)))
            throw new ArgumentException($"Rule with name {ruleInfo.Definition.Name} is already registered");
        
        _rules.Add(ruleInfo);
    }
}
