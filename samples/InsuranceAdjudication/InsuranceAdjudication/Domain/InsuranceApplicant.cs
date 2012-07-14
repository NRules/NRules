namespace NRules.Core.IntegrationTests.Domain
{
    internal class InsuranceApplicant
    {
        private readonly Person _applicant;
        private readonly InsuranceHistory _insuranceHistory;
        private readonly PersonalFinances _personalFinances;

        public InsuranceApplicant(Person applicant,
                                  InsuranceHistory insuranceHistory,
                                  PersonalFinances personalFinances)
        {
            _applicant = applicant;
            _insuranceHistory = insuranceHistory;
            _personalFinances = personalFinances;
        }

        public InsuranceHistory InsuranceHistory
        {
            get { return _insuranceHistory; }
        }

        public Person Applicant
        {
            get { return _applicant; }
        }

        public PersonalFinances PersonalFinances
        {
            get { return _personalFinances; }
        }
    }
}