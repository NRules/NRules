using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NUnit.Framework;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRuleTestFixture
    {
        protected ISession Session;

        private Dictionary<Type, Mock<INotifier>> _notifiers;
        private List<BaseRule> _rules;

        [SetUp]
        public void SetUp()
        {
            _notifiers = new Dictionary<Type, Mock<INotifier>>();
            _rules = new List<BaseRule>();
            SetUpRules();

            var repository = new RuleRepository {Activator = new InstanceActivator(_rules)};
            repository.Load("Test", x => x.From(_rules.Select(r => r.GetType()).ToArray()));

            ISessionFactory factory = repository.Compile();
            Session = factory.CreateSession();
        }

        protected abstract void SetUpRules();

        protected void SetUpRule<T>() where T : BaseRule, new()
        {
            var notifier = new Mock<INotifier>();
            _notifiers.Add(typeof (T), notifier);

            var ruleInstance = new T {Notifier = notifier.Object};
            _rules.Add(ruleInstance);
        }

        protected T GetRuleInstance<T>() where T : BaseRule
        {
            return _rules.OfType<T>().First();
        }

        private Mock<INotifier> GetNotifier<T>()
        {
            return _notifiers.First(n => n.Key == typeof (T)).Value;
        }

        private Mock<INotifier> GetNotifier()
        {
            return _notifiers.First().Value;
        }

        protected void AssertFiredOnce()
        {
            GetNotifier().Verify(x => x.RuleActivated(), Times.Once);
        }

        protected void AssertFiredTwice()
        {
            GetNotifier().Verify(x => x.RuleActivated(), Times.Exactly(2));
        }

        protected void AssertDidNotFire()
        {
            GetNotifier().Verify(x => x.RuleActivated(), Times.Never);
        }

        protected void AssertFiredOnce<T>()
        {
            GetNotifier<T>().Verify(x => x.RuleActivated(), Times.Once);
        }

        protected void AssertFiredTwice<T>()
        {
            GetNotifier<T>().Verify(x => x.RuleActivated(), Times.Exactly(2));
        }

        protected void AssertDidNotFire<T>()
        {
            GetNotifier<T>().Verify(x => x.RuleActivated(), Times.Never);
        }

        private class InstanceActivator : IRuleActivator
        {
            private readonly Dictionary<Type, Rule> _rules;

            public InstanceActivator(IEnumerable<Rule> rules)
            {
                _rules = rules.ToDictionary(r => r.GetType());
            }

            public Rule Activate(Type type)
            {
                return _rules[type];
            }
        }
    }
}