using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.IntegrationTests.TestAssets
{
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
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.Last();
            return x.Facts.Where(f => typeof(T).IsAssignableFrom(f.Declaration.Type)).Select(f => (T) f.Value);
        }

        protected T GetFiredFact<T>()
        {
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.Last();
            return (T)x.Facts.First(f => typeof(T).IsAssignableFrom(f.Declaration.Type)).Value;
        }

        protected T GetFiredFact<T>(int instanceNumber)
        {
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.ElementAt(instanceNumber);
            return (T)x.Facts.First(f => typeof(T).IsAssignableFrom(f.Declaration.Type)).Value;
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
            var ruleEntry = _firedRulesMap.First();
            string ruleName = ruleEntry.Key;
            var actual = ruleEntry.Value.Count;
            AssertRuleFiredTimes(ruleName, expected, actual);
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
            var ruleMetadata = _ruleMap[typeof(T)];
            string ruleName = ruleMetadata.Name;
            var actual = _firedRulesMap[ruleName].Count;
            AssertRuleFiredTimes(ruleName, expected, actual);
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

        private class InstanceActivator : IRuleActivator
        {
            private readonly Dictionary<Type, Rule> _rules = new Dictionary<Type, Rule>();

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
}
