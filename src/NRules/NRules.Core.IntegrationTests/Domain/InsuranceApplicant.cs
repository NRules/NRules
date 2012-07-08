using System;

namespace NRules.Core.IntegrationTests.Domain
{
    internal class InsuranceApplicant
    {
        private readonly string _applicantName;
        private readonly DateTime _applicantBirthDate;
        private readonly Gender _gender;
        private readonly Address _address;
        private readonly PersonalFinances _personalFinances;

        public InsuranceApplicant(string applicantName, 
                                  DateTime applicantBirthDate, 
                                  Gender gender, 
                                  Address address,
                                  PersonalFinances personalFinances)
        {
            _applicantName = applicantName;
            _applicantBirthDate = applicantBirthDate;
            _gender = gender;
            _address = address;
            _personalFinances = personalFinances;
        }

        public PersonalFinances PersonalFinances
        {
            get { return _personalFinances; }
        }

        public string ApplicantName
        {
            get { return _applicantName; }
        }

        public DateTime ApplicantBirthDate
        {
            get { return _applicantBirthDate; }
        }

        public int ApplicantAge
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - ApplicantBirthDate.Year;
            
                if (ApplicantBirthDate > today.AddYears(-age)) 
                    age--;

                return age;
            }
        }

        public Gender Gender
        {
            get { return _gender; }
        }

        public Address Address
        {
            get { return _address; }
        }
    }
}
