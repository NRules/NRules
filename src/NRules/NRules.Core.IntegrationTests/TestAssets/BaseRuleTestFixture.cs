using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Config;
using NRules.Dsl;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Core.IntegrationTests.TestAssets
{
    public abstract class BaseRuleTestFixture
    {
        protected ISession Session;

        private IContainer _container;
        private RuleRepository _repository;
        private Dictionary<Type, INotifier> _notifiers;
        private List<BaseRule> _rules;
        private IRuleCompiler _compiler;

        [SetUp]
        public void SetUp()
        {
            _container = MockRepository.GenerateStub<IContainer>();
            _container.Stub(x => x.CreateChildContainer()).Return(_container);
            _compiler = new RuleCompiler();
            _repository = new RuleRepository();
            _repository.Container = _container;
            _notifiers = new Dictionary<Type, INotifier>();
            _rules = new List<BaseRule>();

            SetUpRules();
            _container.Stub(x => x.BuildAll(typeof (IRule))).Return(_rules);

            _repository.AddRuleSet(_rules.Select(r => r.GetType()).ToArray());

            ISessionFactory factory = _compiler.Compile(_repository.Rules);
            Session = factory.CreateSession();
        }

        protected abstract void SetUpRules();

        protected void SetUpRule<T>() where T : BaseRule, new()
        {
            var notifier = MockRepository.GenerateStub<INotifier>();
            _notifiers.Add(typeof (T), notifier);

            var ruleInstance = new T() {Notifier = notifier};
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
    }
}