using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

public class RuleTestFixture
{
    private readonly Dictionary<string, List<IMatch>> _firedRulesMap = new();
    private readonly Dictionary<Type, IRuleMetadata> _ruleMap = new();
    private readonly Lazy<ISession> _lazySession;
    private readonly RuleCompiler _compiler;

    public RuleTestFixture(IRuleActivator ruleActivator, RuleCompiler compiler)
    {
        Repository = new RuleRepository
        {
            Activator = ruleActivator
        };
        _compiler = compiler;

        Setup = new RepositorySetup(Repository, _ruleMap);

        _lazySession = new(CreateSession);

        ISession CreateSession()
        {
            var ruleDefinitions = Repository.GetRuleSets().SelectMany(set => set.Rules);
            var factory = _compiler.Compile(ruleDefinitions);
            var session = factory.CreateSession();
            session.Events.RuleFiredEvent += (_, args) => GetFiredRules(args.Rule.Name).Add(args.Match);
            return session;
        }
    }

    public RuleRepository Repository { get; }

    public ISession Session => _lazySession.Value;

    public IRepositorySetup Setup { get; }

    public T GetRuleInstance<T>()
        where T : Rule =>
        Repository.Activator.Activate<T>().Single();

    public IEnumerable<T> GetFiredFacts<T>()
    {
        var ruleMetadata = GetRule();
        var matches = GetFiredRules(ruleMetadata.Name);
        var match = matches.LastOrDefault()
            ?? throw new InvalidOperationException($"Unable to retrieve last firing of the rule {ruleMetadata.Name}");

        return GetFacts<T>(match).Select(f => f.Value).Cast<T>();
    }

    public T GetFiredFact<T>()
    {
        var ruleMetadata = GetRule();
        var matches = GetFiredRules(ruleMetadata.Name);
        var match = matches.LastOrDefault()
            ?? throw new InvalidOperationException($"Unable to retrieve last firing of the rule {ruleMetadata.Name}");

        var factMatchess = GetFacts<T>(match);
        var factMatch = factMatchess.FirstOrDefault()
            ?? throw new InvalidOperationException($"Did not find any facts of type {typeof(T)} in the rule match");
        return (T)factMatch.Value;
    }

    public T GetFiredFact<T>(int index)
    {
        var ruleMetadata = GetRule();
        var ruleFirings = GetFiredRules(ruleMetadata.Name);
        if (ruleFirings.Count < index + 1)
            throw new InvalidOperationException($"Unable to retrieve firing of the rule {ruleMetadata.Name} at index {index}");

        var match = ruleFirings.ElementAt(index);
        var factMatchess = GetFacts<T>(match);
        var factMatch = factMatchess.FirstOrDefault()
            ?? throw new InvalidOperationException($"Did not find any facts of type {typeof(T)} in the rule match");
        return (T)factMatch.Value;
    }

    public IRuleFireAssertResult IsFiredTimes(int expected) =>
        GetAssertResult(GetRule(), expected);

    public IRuleFireAssertResult IsFiredTimes<T>(int expected)
        where T : Rule =>
        GetAssertResult(GetRule<T>(), expected);

    private IRuleFireAssertResult GetAssertResult(IRuleMetadata ruleMetadata, int expected)
        => new RuleFireAssertResult(ruleMetadata, expected, GetFiredRules(ruleMetadata.Name).Count);

    private IRuleMetadata GetRule() =>
        _ruleMap.Count switch
        {
            0 => throw new ArgumentException("Expected single rule test, but found no rules registered"),
            1 => _ruleMap.Values.First(),
            _ => throw new ArgumentException("Expected single rule test, but found multiple rules registered"),
        };

    private IRuleMetadata GetRule<T>()
        where T : Rule =>
        _ruleMap.TryGetValue(typeof(T), out var ruleMetadata)
            ? ruleMetadata
            : throw new ArgumentException($"Rule of type {typeof(T).FullName} was not found");

    private static IEnumerable<IFactMatch> GetFacts<T>(IMatch match) => match.Facts.Where(f => typeof(T).IsAssignableFrom(f.Declaration.Type));

    private List<IMatch> GetFiredRules(string name)
    {
        if (_firedRulesMap.TryGetValue(name, out var matches))
        {
            return matches;
        }

        return _firedRulesMap[name] = new();
    }
}
