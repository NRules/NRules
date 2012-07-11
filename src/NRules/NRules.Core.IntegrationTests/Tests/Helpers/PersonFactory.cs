using System;
using NRules.Core.IntegrationTests.Domain;

namespace NRules.Core.IntegrationTests.Tests.Helpers
{
    internal class PersonFactory
    {
        public static Person YoungPerson()
        {
            var person = new Person("Joe Bob", DateTime.Parse("11/07/2002"), Gender.Male, AddressFactory.StandardAddress());
            return person;
        }

        public static Person MiddledAgedPerson()
        {
            var person = new Person("John Teller", DateTime.Parse("11/07/1978"), Gender.Male, AddressFactory.StandardAddress());
            return person;
        }
    }
}
