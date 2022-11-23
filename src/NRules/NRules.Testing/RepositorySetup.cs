using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

internal sealed class RepositorySetup : IRepositorySetup, IRuleAccessor
{
    private readonly RuleRepository _repository;
    private readonly Dictionary<Type, IRuleMetadata> _ruleMap = new();
    private readonly Dictionary<string, List<IMatch>> _firedRulesMap = new();

    public RepositorySetup(RuleRepository repository) => _repository = repository;

    public void OnRuleFired(IMatch match) => InternalGetFiredRuleMatches(match.Rule.Name).Add(match);

    public IReadOnlyCollection<IMatch> GetFiredRuleMatches(string ruleName) => InternalGetFiredRuleMatches(ruleName);

    public IRuleMetadata GetRule() =>
        _ruleMap.Count switch
        {
            0 => throw new ArgumentException("Expected single rule test, but found no rules registered"),
            1 => _ruleMap.Values.First(),
            _ => throw new ArgumentException("Expected single rule test, but found multiple rules registered"),
        };

    public IRuleMetadata GetRule<T>()
        where T : Rule =>
        _ruleMap.TryGetValue(typeof(T), out var ruleMetadata)
            ? ruleMetadata
            : throw new ArgumentException($"Rule of type {typeof(T).FullName} was not found");

    public void Rule<T>()
        where T : Rule =>
        Rule(typeof(T));

    public void Rule(Type ruleType) =>
        Rule(new RuleMetadata(ruleType));

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
