using System;

namespace NRules.Core.IntegrationTests.Domain
{
    internal class QualificationEventArgs : EventArgs
    {
        public QualificationEventArgs(InsuranceApplicant applicant)
        {
            Applicant = applicant;
        }

        public InsuranceApplicant Applicant { get; internal set; }
    }
}