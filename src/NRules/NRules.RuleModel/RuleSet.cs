using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Represents a named set of rules.
/// </summary>
public interface IRuleSet
{
    /// <summary>
    /// Rule set name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Adds rules to the rule set.
    /// </summary>
    /// <param name="ruleDefinitions">Rule definitions to add.</param>
    void Add(IEnumerable<IRuleDefinition> ruleDefinitions);

    /// <summary>
    /// Rules in the rule set.
    /// </summary>
    IEnumerable<IRuleDefinition> Rules { get; }
}

/// <summary>
/// Default implementation of a rule set.
/// </summary>
public class RuleSet(string name) : IRuleSet
{
    private readonly List<IRuleDefinition> _rules = new();

    public string Name { get; } = name;
    public IEnumerable<IRuleDefinition> Rules => _rules;

    public void Add(IEnumerable<IRuleDefinition> ruleDefinitions)
    {
        _rules.AddRange(ruleDefinitions);
    }
}