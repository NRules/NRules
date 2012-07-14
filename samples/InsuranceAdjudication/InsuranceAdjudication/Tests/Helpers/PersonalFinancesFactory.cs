using NRules.Core.IntegrationTests.Domain;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    internal class PersonalFinancesFactory
    {
        public static PersonalFinances PoorAndNotInDebt()
        {
            return new PersonalFinances(10.25m, 0m, 12500.0m, 1052m);
        }

        public static PersonalFinances PoorAndInDebt()
        {
            return new PersonalFinances(150.0m, 0m, 48000.0m, 100752m);
        }

        public static PersonalFinances RichAndInDebt()
        {
            return new PersonalFinances(18400.0m, 125789.55m, 125000.0m, 50200m);
        }

        public static PersonalFinances RichAndNoDebt()
        {
            return new PersonalFinances(18400.0m, 125789.55m, 125000.0m, 1500m);
        }
    }
}