using System;
using System.Collections.Generic;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

internal class RepositorySetup : IRepositorySetup
{
    private readonly RuleRepository _repository;
    private readonly IDictionary<Type, IRuleMetadata> _ruleMap;

    public RepositorySetup(RuleRepository repository, IDictionary<Type, IRuleMetadata> ruleMap)
    {
        _repository = repository;
        _ruleMap = ruleMap;
    }

    public void Rule<T>()
        where T : Rule =>
        Rule(new RuleMetadata(typeof(T)));

    public void Rule(IRuleMetadata metadata)
    {
        _ruleMap[metadata.RuleType] = metadata;
        _repository.Load(x => x
            .PrivateTypes()
            .NestedTypes()
            .From(metadata.RuleType));
    }
}
