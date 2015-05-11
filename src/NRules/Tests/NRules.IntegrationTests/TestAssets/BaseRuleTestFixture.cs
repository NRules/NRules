using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NUnit.Framework;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRuleTestFixture
    {
        protected ISession Session;
        protected RuleRepository Repository;

        private Dictionary<string, List<AgendaEventArgs>> _firedRulesMap;
        private Dictionary<Type, IRuleMetadata> _ruleMap;

        [SetUp]
        public void SetUp()
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

        protected void SetUpRule<T>() where T : BaseRule, new()
        {
            var metadata = new RuleMetadata(typeof (T));
            _ruleMap[typeof (T)] = metadata;
            _firedRulesMap[metadata.Name] = new List<AgendaEventArgs>();
            Repository.Load(x => x.From(typeof(T)));
        }

        protected T GetRuleInstance<T>() where T : BaseRule
        {
            return (T)Repository.Activator.Activate(typeof(T));
        }

        protected T GetFiredFact<T>()
        {
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.Last();
            return (T)x.Facts.First(f => typeof(T).IsAssignableFrom(f.Type)).Value;
        }

        protected T GetFiredFact<T>(int instanceNubmer)
        {
            var rule = _ruleMap.Single().Value;
            var firedRule = _firedRulesMap[rule.Name];
            var x = firedRule.ElementAt(instanceNubmer);
            return (T)x.Facts.First(f => typeof(T).IsAssignableFrom(f.Type)).Value;
        }

        protected void AssertFiredOnce()
        {
            Assert.AreEqual(1, _firedRulesMap.First().Value.Count);
        }

        protected void AssertFiredTwice()
        {
            Assert.AreEqual(2, _firedRulesMap.First().Value.Count);
        }

        protected void AssertFiredTimes(int value)
        {
            Assert.AreEqual(value, _firedRulesMap.First().Value.Count);
        }

        protected void AssertDidNotFire()
        {
            Assert.AreEqual(0, _firedRulesMap.First().Value.Count);
        }

        protected void AssertFiredOnce<T>()
        {
            var rule = _ruleMap[typeof (T)];
            Assert.AreEqual(1, _firedRulesMap[rule.Name].Count);
        }

        protected void AssertFiredTwice<T>()
        {
            var rule = _ruleMap[typeof(T)];
            Assert.AreEqual(2, _firedRulesMap[rule.Name].Count);
        }

        protected void AssertDidNotFire<T>()
        {
            var rule = _ruleMap[typeof(T)];
            Assert.AreEqual(0, _firedRulesMap[rule.Name].Count);
        }

        private class InstanceActivator : IRuleActivator
        {
            private readonly Dictionary<Type, Rule> _rules = new Dictionary<Type, Rule>();

            public Rule Activate(Type type)
            {
                Rule rule;
                if (!_rules.TryGetValue(type, out rule))
                {
                    rule = (Rule) Activator.CreateInstance(type);
                    _rules[type] = rule;
                }
                return rule;
            }
        }
    }
}