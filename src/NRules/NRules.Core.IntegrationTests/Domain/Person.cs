using System;

namespace NRules.Core.IntegrationTests.Domain
{
    internal class Person
    {
        private readonly string _name;
        private readonly DateTime _birthDate;
        private readonly Gender _gender;
        private readonly Address _address;

        public Person(string name, DateTime birthDate, Gender gender, Address address)
        {
            _name = name;
            _birthDate = birthDate;
            _gender = gender;
            _address = address;
        }

        public string Name
        {
            get { return _name; }
        }

        public DateTime BirthDate
        {
            get { return _birthDate; }
        }

        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - BirthDate.Year;
            
                if (BirthDate > today.AddYears(-age)) 
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
