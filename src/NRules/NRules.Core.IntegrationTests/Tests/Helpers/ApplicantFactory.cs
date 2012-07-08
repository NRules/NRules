using System;
using NRules.Core.IntegrationTests.Domain;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    internal class ApplicantFactory
    {
        public static InsuranceApplicant YoungApplicant(PersonalFinances finances)
        {
            var applicant = new InsuranceApplicant("Joe Bob",
                                              DateTime.Parse("11/07/2002"),
                                              Gender.Male,
                                              AddressFactory.StandardAddress(),
                                              finances);
            return applicant;
        }

        public static InsuranceApplicant MiddleAgedApplicant(PersonalFinances finances)
        {
            var applicant = new InsuranceApplicant("John Teller",
                                              DateTime.Parse("11/07/1978"),
                                              Gender.Male,
                                              AddressFactory.StandardAddress(),
                                              finances);
            return applicant;
        }
    }
}
