using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.IntegrationTests.TestAssets;

public abstract class BaseRuleTestFixture
{
    protected readonly ISession Session;
    protected readonly RuleRepository Repository;

    private readonly Dictionary<string, List<AgendaEventArgs>> _firedRulesMap;
    private readonly Dictionary<Type, IRuleMetadata> _ruleMap;

    protected BaseRuleTestFixture()
    {
        _firedRulesMap = new Dictionary<string, List<AgendaEventArgs>>();
        _ruleMap = new Dictionary<Type, IRuleMetadata>();

        Repository = new RuleRepository {Activator = new InstanceActivator()};

        SetUpRules();

        ISessionFactory factory = Compile();
        Session = factory.CreateSession();
        Session.Events.RuleFiredEvent += (sender, args) => _firedRulesMap[args.Rule.Name].Add(args);
    }

    protected virtual ISessionFactory Compile()
    {
        var factory = Repository.Compile();
        return factory;
    }

    protected abstract void SetUpRules();

    protected void SetUpRule<T>() where T : Rule, new()
    {
        var metadata = new RuleMetadata(typeof (T));
        _ruleMap[typeof (T)] = metadata;
        _firedRulesMap[metadata.Name] = new List<AgendaEventArgs>();
        Repository.Load(x => x
            .PrivateTypes()
            .NestedTypes()
            .From(typeof (T)));
    }

    protected T GetRuleInstance<T>() where T : Rule
    {
        return (T)Repository.Activator.Activate(typeof(T)).Single();
    }

    protected IEnumerable<T> GetFiredFacts<T>()
    {
        var ruleFiring = GetLastRuleFiring();
        return GetFacts<T>(ruleFiring, allowEmpty: true).Select(f => (T) f.Value);
    }

    protected T GetFiredFact<T>()
    {
        var ruleFiring = GetLastRuleFiring();
        return (T)GetFacts<T>(ruleFiring).First().Value;
    }

    protected T GetFiredFact<T>(int instanceNumber)
    {
        var ruleFiring = GetRuleFiring(instanceNumber);
        return (T)GetFacts<T>(ruleFiring).First().Value;
    }

    protected void AssertFiredOnce()
    {
        AssertFiredTimes(1);
    }

    protected void AssertFiredTwice()
    {
        AssertFiredTimes(2);
    }

    protected void AssertFiredTimes(int expected)
    {
        var ruleMetadata = GetRule();
        var actual = GetRuleFirings(ruleMetadata).Count;
        AssertRuleFiredTimes(ruleMetadata.Name, expected, actual);
    }

    protected void AssertDidNotFire()
    {
        AssertFiredTimes(0);
    }

    protected void AssertFiredOnce<T>()
    {
        AssertFiredTimes<T>(1);
    }

    protected void AssertFiredTwice<T>()
    {
        AssertFiredTimes<T>(2);
    }

    protected void AssertFiredTimes<T>(int expected)
    {
        var ruleMetadata = GetRule<T>();
        var actual = GetRuleFirings(ruleMetadata).Count;
        AssertRuleFiredTimes(ruleMetadata.Name, expected, actual);
    }
    
    protected void AssertDidNotFire<T>()
    {
        AssertFiredTimes<T>(0);
    }
    
    private static void AssertRuleFiredTimes(string ruleName, int expected, int actual)
    {
        if (expected != actual)
            throw new RuleFiredAssertionException(expected, actual, ruleName);
    }

    private IRuleMetadata GetRule<T>()
    {
        if (!_ruleMap.TryGetValue(typeof(T), out var ruleMetadata))
            throw new ArgumentException($"Rule {typeof(T).FullName} not found");
        return ruleMetadata;
    }
    
    private IRuleMetadata GetRule()
    {
        if (_ruleMap.Count == 0)
            throw new ArgumentException("Expected single rule test, but found no rules registered");
        if (_ruleMap.Count > 1)
            throw new ArgumentException("Expected single rule test, but found multiple rules registered");

        return _ruleMap.Single().Value;
    }

    private IReadOnlyCollection<AgendaEventArgs> GetRuleFirings(IRuleMetadata ruleMetadata)
    {
        if (!_firedRulesMap.TryGetValue(ruleMetadata.Name, out var firings))
            throw new ArgumentException($"Expected rule {ruleMetadata.Name} to fire");
        return firings;
    }

    private AgendaEventArgs GetLastRuleFiring()
    {
        var ruleMetadata = GetRule();
        var ruleFirings = GetRuleFirings(ruleMetadata);
        var lastFiring = ruleFirings.LastOrDefault();
        if (lastFiring == null)
            throw new InvalidOperationException($"Unable to retrieve last firing of the rule {ruleMetadata.Name}");
        return lastFiring;
    }

    private AgendaEventArgs GetRuleFiring(int firingIndex)
    {
        var ruleMetadata = GetRule();
        var ruleFirings = GetRuleFirings(ruleMetadata);
        if (ruleFirings.Count < firingIndex + 1)
            throw new InvalidOperationException($"Unable to retrieve firing of the rule {ruleMetadata.Name} at index {firingIndex}");
        return ruleFirings.ElementAt(firingIndex);
    }

    private static IFactMatch[] GetFacts<T>(AgendaEventArgs ruleFiring, bool allowEmpty = false)
    {
        var factMatches = ruleFiring.Facts.Where(f => typeof(T).IsAssignableFrom(f.Declaration.Type)).ToArray();
        if (factMatches.Length == 0 && !allowEmpty)
            throw new InvalidOperationException($"Did not find any facts of type {typeof(T)} in the rule match");
        return factMatches;
    }

    private class InstanceActivator : IRuleActivator
    {
        private readonly Dictionary<Type, Rule> _rules = new();

        public IEnumerable<Rule> Activate(Type type)
        {
            if (!_rules.TryGetValue(type, out var rule))
            {
                rule = (Rule) Activator.CreateInstance(type);
                _rules[type] = rule;
            }
            yield return rule;
        }
    }
}
