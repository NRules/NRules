using NRules.Core.IntegrationTests.TestAssets;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Core.IntegrationTests
{
    public class BaseRuleTestFixture<T> where T : BaseRule, new()
    {
        protected ISession Session;
        protected IContainer Container;
        protected INotifier Notifier;
        protected T RuleInstance;

        [SetUp]
        public void SetUp()
        {
            Notifier = MockRepository.GenerateStub<INotifier>();

            Container = MockRepository.GenerateStub<IContainer>();
            RuleInstance = new T() {Notifier = Notifier};
            Container.Stub(x => x.GetObjectInstance(typeof (T)))
                .Return(RuleInstance);

            var repository = new RuleRepository(Container);
            repository.AddRuleSet(typeof (T));

            var factory = new SessionFactory(repository);
            Session = factory.CreateSession();
        }

        protected void AssertFiredOnce()
        {
            Notifier.AssertWasCalled(x => x.RuleActivated(), c => c.Repeat.Once());
        }

        protected void AssertFiredTwice()
        {
            Notifier.AssertWasCalled(x => x.RuleActivated(), c => c.Repeat.Twice());
        }

        protected void AssertDidNotFire()
        {
            Notifier.AssertWasNotCalled(x => x.RuleActivated());
        }
    }
}