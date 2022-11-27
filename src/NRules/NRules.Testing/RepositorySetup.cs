using System;
using System.Collections.Generic;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

internal sealed class RepositorySetup : IRepositorySetup, IRuleAccessor
{
    private readonly RuleCompiler _compiler;
    private readonly RuleRepository _repository;
    private readonly Dictionary<Type, IRuleMetadata> _ruleMap = new();
    private readonly Dictionary<string, List<IMatch>> _firedRulesMap = new();

    public RepositorySetup(RuleCompiler compiler, RuleRepository repository)
    {
        _compiler = compiler;
        _repository = repository;
    }

    public IReadOnlyCollection<Type> RegisteredRuleTypes => _ruleMap.Keys;

    public RuleCompiler Compiler => _compiler;

    public IRuleMetadata GetRule(Type ruleType) =>
        _ruleMap.TryGetValue(ruleType, out var ruleMetadata)
            ? ruleMetadata
            : throw new ArgumentException($"Rule of type {ruleType.FullName} was not registered");

    public IReadOnlyCollection<IMatch> GetFiredRuleMatches(string ruleName) => InternalGetFiredRuleMatches(ruleName);

    public void Rule<T>()
        where T : Rule =>
        Rule(typeof(T));

    public void Rule(Type ruleType) =>
        Rule(new RuleMetadata(ruleType));

    public void OnRuleFired(IMatch match) => InternalGetFiredRuleMatches(match.Rule.Name).Add(match);

    private void Rule(IRuleMetadata metadata)
    {
        _ruleMap[metadata.RuleType] = metadata;
        _repository.Load(x => x
            .PrivateTypes()
            .NestedTypes()
            .From(metadata.RuleType));
    }

    private List<IMatch> InternalGetFiredRuleMatches(string ruleName) => _firedRulesMap.GetOrAdd(ruleName, _ => new());
}
