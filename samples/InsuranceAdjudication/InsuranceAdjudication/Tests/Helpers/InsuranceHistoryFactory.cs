using System;
using System.Collections.Generic;
using NRules.Core.IntegrationTests.Domain;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    internal class InsuranceHistoryFactory
    {
        public static InsuranceHistory StandardInsurancePolicies(Person beneficiary)
        {
            var b = new Beneficiary(beneficiary);
            var policies = new List<InsurancePolicy>();
            var policy = new InsurancePolicy(DateTime.Parse("7/1/2012"), DateTime.Today.AddDays(1), PolicyType.Life, b,
                                             true, 125, 500);
            policies.Add(policy);
            var history = new InsuranceHistory(policies, 3, 3500, 34000);
            return history;
        }
    }
}