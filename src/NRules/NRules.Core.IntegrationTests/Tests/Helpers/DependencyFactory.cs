using System;
using NRules.Core.IntegrationTests.Rules;
using Rhino.Mocks;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    internal class DependencyFactory
    {
        public static IContainer GetContainer(EventHandler handler)
        {
            var container = MockRepository.GenerateStub<IContainer>();
            container.Stub(x => x.GetObjectInstance(typeof(SimplePersonalFinancesRule))).Return(new SimplePersonalFinancesRule(handler));
            container.Stub(x => x.GetObjectInstance(typeof(SelfBeneficialPolicyRule))).Return(new SelfBeneficialPolicyRule(handler));
            return container;
        }
    }
}
