using System;

namespace NRules.Core.IntegrationTests.Domain
{
    internal class InsurancePolicy
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly PolicyType _type;
        private readonly Beneficiary _beneficiary;
        private readonly bool _mustHaveSeperateBeneficiary;
        private readonly decimal _premium;
        private readonly decimal _deductable;

        public InsurancePolicy(DateTime startDate,
                               DateTime endDate,
                               PolicyType type,
                               Beneficiary beneficiary,
                               bool mustHaveSeperateBeneficiary,
                               decimal premium,
                               decimal deductable)
        {
            _startDate = startDate;
            _endDate = endDate;
            _type = type;
            _beneficiary = beneficiary;
            _mustHaveSeperateBeneficiary = mustHaveSeperateBeneficiary;
            _premium = premium;
            _deductable = deductable;
        }

        public DateTime StartDate
        {
            get { return _startDate; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
        }

        public PolicyType Type
        {
            get { return _type; }
        }

        public Beneficiary Beneficiary
        {
            get { return _beneficiary; }
        }

        public bool MustHaveSeperateBeneficiary
        {
            get { return _mustHaveSeperateBeneficiary; }
        }

        public decimal Premium
        {
            get { return _premium; }
        }

        public decimal Deductable
        {
            get { return _deductable; }
        }
    }
}