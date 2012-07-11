namespace NRules.Core.IntegrationTests.Domain
{
    internal class Beneficiary
    {
        private readonly Person _beneficiary;

        public Beneficiary(Person beneficiary)
        {
            _beneficiary = beneficiary;
        }

        public Person TheBeneficiary
        {
            get { return _beneficiary; }
        }
    }
}
