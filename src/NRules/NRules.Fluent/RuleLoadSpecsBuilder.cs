using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Fluent;

/// <summary>
/// Rule Bulk loader specification interface.
/// Responsible for:
/// - Specifing multiple sets of rule(types) and deliver does types and ruleSetNames to RuleRepository.
/// - Optionally deliver types to other targets like e.g. Dependency injection.
/// </summary>
public interface IRuleBulkLoadSpec
{
    IRuleBulkLoadSpec Specify(string ruleSetName, Action<IRuleTypeScanner> scanAction);
    
    IRuleBulkLoadSpec Load(IRuleActivator ruleActivator);

    public IEnumerable<Type> GetRuleTypes();

    public IEnumerable<Type> GetRuleTypes(params string[] ruleSetNames);

    /// <summary>
    /// Get all RuleSets as specified by this Bulk loader specification.
    /// </summary>
    /// <returns>All instances of IRuleSet found by this Bulk loader.</returns>
    public IEnumerable<IRuleSet> GetRuleSets();

}

/// <summary>
/// Rule Bulk loader specification class.
/// Responsible for:
/// - Specifing multiple sets of rule(types) and deliver does types and ruleSetNames to RuleRepository.
/// - Optionally deliver types to other targets like e.g. Dependency injection.
/// </summary>
internal class RuleBulkLoadSpec : IRuleBulkLoadSpec
{
    internal class BulkSet
    {
        public string RuleSetName { get; set; } = string.Empty;
        public Type[]? RuleTypes { get; set; }
        public RuleSet? RuleSet { get; set; }
    }
        
    private readonly Dictionary<string, BulkSet> _bulkDictionary;
    
    public RuleBulkLoadSpec()
    {
        this._bulkDictionary = new Dictionary<string, BulkSet>();
    }

    public IRuleBulkLoadSpec Specify(string ruleSetName, Action<IRuleTypeScanner> scanAction)
    {
        var scanner = new RuleTypeScanner();
        scanAction(scanner);

        var bulkSet = new BulkSet
        {
            RuleSetName = ruleSetName,
            RuleTypes = scanner.GetRuleTypes()
        };

        if (_bulkDictionary.ContainsKey(bulkSet.RuleSetName))
        {
            throw new ArgumentException($"Rule set with the same name already exists. Name={ruleSetName}");
        }

        _bulkDictionary[ruleSetName] = bulkSet;

        return this;
    }
    
    public IRuleBulkLoadSpec Load(IRuleActivator ruleActivator)
    {
        foreach (var bulkSet in _bulkDictionary.Values)
        {
            var spec = new RuleLoadSpec(ruleActivator);
            spec.From(bulkSet.RuleTypes!).To(bulkSet.RuleSetName);
            var ruleDefinitions = spec.Load();

            var set = new RuleSet(bulkSet.RuleSetName);
            set.Add(ruleDefinitions);

            this._bulkDictionary[bulkSet.RuleSetName].RuleSet = set;
        }

        return this;
    }

    public IEnumerable<Type> GetRuleTypes()
    {
        return this._bulkDictionary
            .SelectMany(bulkSet => bulkSet.Value.RuleTypes);
    }
    
    public IEnumerable<Type> GetRuleTypes(params string[] ruleSetNames)
    {
        return this._bulkDictionary
            .Where(bulkSet => ruleSetNames.Contains(bulkSet.Key))
            .SelectMany(bulkSet => bulkSet.Value.RuleTypes);
    }

    /// <summary>
    /// Get all RuleSets as specified by this Bulk loader specification.
    /// </summary>
    /// <returns>All instances of IRuleSet found by this Bulk loader.</returns>
    public IEnumerable<IRuleSet> GetRuleSets()
    {
        return this._bulkDictionary
            .Select(bulkSet => bulkSet.Value.RuleSet!);
    }
}

/// <summary>
/// Builder for internal RuleBulkLoadSpecBuilder class.
/// </summary>
public class RuleBulkLoadSpecBuilder
{
    private IRuleBulkLoadSpec _ruleBulkLoadSpec;
    public RuleBulkLoadSpecBuilder()
    {
        this._ruleBulkLoadSpec = new RuleBulkLoadSpec();
    }

    public RuleBulkLoadSpecBuilder Specify(string ruleSetName, Action<IRuleTypeScanner> scanAction)
    {
        this._ruleBulkLoadSpec.Specify(ruleSetName, scanAction);
        return this;
    }

    public IRuleBulkLoadSpec Build()
    {
        return this._ruleBulkLoadSpec;
    }
}
