using NRules.Core.IntegrationTests.Domain;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    internal class AddressFactory
    {
        public static Address StandardAddress()
        {
            return new Address(1234, "Rules Boulevard", null, "Denver", "Colorado", 80202);
        }
    }
}
