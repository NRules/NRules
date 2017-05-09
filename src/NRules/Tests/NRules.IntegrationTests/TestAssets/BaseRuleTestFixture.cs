using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using Xunit;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRuleTestFixture
    {
        protected ISession Session;
        protected RuleRepository Repository;

        private Dictionary<string, List<AgendaEventArgs>> _firedRulesMap;
        private Dictionary<Type, IRuleMetadata> _ruleMap;

        protected BaseRuleTestFixture()
        {
            _firedRulesMap = new Dictionary<string, List<AgendaEventArgs>>();
            _ruleMap = new Dictionary<Type, IRuleMetadata>();

            Repository = new RuleRepository {Activator = new InstanceActivator()};

            SetUpRules();

            ISessionFactory factory = Repository.Compile();
            Session = factory.CreateSession();
            Session.Events.RuleFiredEvent += (sender, args) => _firedRulesMap[args.Rule.Name].Add(args);
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

        protected T GetFiredFact<T>()
        {
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.Last();
            return (T)x.Facts.First(f => typeof(T).GetTypeInfo().IsAssignableFrom(f.Type.GetTypeInfo())).Value;
        }

        protected T GetFiredFact<T>(int instanceNumber)
        {
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.ElementAt(instanceNumber);
            return (T)x.Facts.First(f => typeof(T).GetTypeInfo().IsAssignableFrom(f.Type.GetTypeInfo())).Value;
        }

        protected void AssertFiredOnce()
        {
            Assert.Equal(1, _firedRulesMap.First().Value.Count);
        }

        protected void AssertFiredTwice()
        {
            Assert.Equal(2, _firedRulesMap.First().Value.Count);
        }

        protected void AssertFiredTimes(int value)
        {
            Assert.Equal(value, _firedRulesMap.First().Value.Count);
        }

        protected void AssertDidNotFire()
        {
            Assert.Equal(0, _firedRulesMap.First().Value.Count);
        }

        protected void AssertFiredOnce<T>()
        {
            var rule = _ruleMap[typeof (T)];
            Assert.Equal(1, _firedRulesMap[rule.Name].Count);
        }

        protected void AssertFiredTwice<T>()
        {
            var rule = _ruleMap[typeof(T)];
            Assert.Equal(2, _firedRulesMap[rule.Name].Count);
        }

        protected void AssertDidNotFire<T>()
        {
            var rule = _ruleMap[typeof(T)];
            Assert.Equal(0, _firedRulesMap[rule.Name].Count);
        }

        private class InstanceActivator : IRuleActivator
        {
            private readonly Dictionary<Type, Rule> _rules = new Dictionary<Type, Rule>();

            public IEnumerable<Rule> Activate(Type type)
            {
                Rule rule;
                if (!_rules.TryGetValue(type, out rule))
                {
                    rule = (Rule) Activator.CreateInstance(type);
                    _rules[type] = rule;
                }
                yield return rule;
            }
        }
    }
}
