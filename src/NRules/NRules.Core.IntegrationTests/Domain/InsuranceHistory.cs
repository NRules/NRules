using System.Collections.Generic;

namespace NRules.Core.IntegrationTests.Domain
{
    internal class InsuranceHistory
    {
        private readonly IEnumerable<InsurancePolicy> _policies;
        private readonly int _claims;
        private readonly decimal _totalPayout;
        private readonly decimal _totalPremiums;

        public InsuranceHistory(IEnumerable<InsurancePolicy> policies, int claims, decimal totalPayout, decimal totalPremiums)
        {
            _policies = policies;
            _claims = claims;
            _totalPayout = totalPayout;
            _totalPremiums = totalPremiums;
        }

        public IEnumerable<InsurancePolicy> Policies
        {
            get { return _policies; }
        }

        public int Claims
        {
            get { return _claims; }
        }

        public decimal TotalPayout
        {
            get { return _totalPayout; }
        }

        public decimal TotalPremiums
        {
            get { return _totalPremiums; }
        }
    }
}
