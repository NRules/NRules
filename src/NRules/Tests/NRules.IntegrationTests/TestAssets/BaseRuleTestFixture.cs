using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRuleTestFixture
    {
        protected ISession Session;

        private Dictionary<Type, INotifier> _notifiers;
        private List<BaseRule> _rules;

        [SetUp]
        public void SetUp()
        {
            _notifiers = new Dictionary<Type, INotifier>();
            _rules = new List<BaseRule>();
            SetUpRules();

            var repository = new RuleRepository {Activator = new InstanceActivator(_rules)};
            repository.Add(_rules);

            ISessionFactory factory = repository.Compile();
            Session = factory.CreateSession();
        }

        protected abstract void SetUpRules();

        protected void SetUpRule<T>() where T : BaseRule, new()
        {
            var notifier = MockRepository.GenerateStub<INotifier>();
            _notifiers.Add(typeof (T), notifier);

            var ruleInstance = new T {Notifier = notifier};
            _rules.Add(ruleInstance);
        }

        protected T GetRuleInstance<T>() where T : BaseRule
        {
            return _rules.OfType<T>().First();
        }

        private INotifier GetNotifier<T>()
        {
            return _notifiers.First(n => n.Key == typeof (T)).Value;
        }

        private INotifier GetNotifier()
        {
            return _notifiers.First().Value;
        }

        protected void AssertFiredOnce()
        {
            GetNotifier().AssertWasCalled(x => x.RuleActivated(), c => c.Repeat.Once());
        }

        protected void AssertFiredTwice()
        {
            GetNotifier().AssertWasCalled(x => x.RuleActivated(), c => c.Repeat.Twice());
        }

        protected void AssertDidNotFire()
        {
            GetNotifier().AssertWasNotCalled(x => x.RuleActivated());
        }

        protected void AssertFiredOnce<T>()
        {
            GetNotifier<T>().AssertWasCalled(x => x.RuleActivated(), c => c.Repeat.Once());
        }

        protected void AssertFiredTwice<T>()
        {
            GetNotifier<T>().AssertWasCalled(x => x.RuleActivated(), c => c.Repeat.Twice());
        }

        protected void AssertDidNotFire<T>()
        {
            GetNotifier<T>().AssertWasNotCalled(x => x.RuleActivated());
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