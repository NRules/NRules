namespace NRules.Core.IntegrationTests.Domain
{
    internal class Address
    {
        private readonly int _houseNumber;
        private readonly string _streetName;
        private readonly int? _apartmentNumber;
        private readonly string _city;
        private readonly string _state;
        private readonly int _zipCode;

        public Address(int houseNumber, string streetName, int? apartmentNumber, string city, string state, int zipCode)
        {
            _houseNumber = houseNumber;
            _streetName = streetName;
            _apartmentNumber = apartmentNumber;
            _city = city;
            _state = state;
            _zipCode = zipCode;
        }

        public int ZipCode
        {
            get { return _zipCode; }
        }

        public string State
        {
            get { return _state; }
        }

        public string City
        {
            get { return _city; }
        }

        public int? ApartmentNumber
        {
            get { return _apartmentNumber; }
        }

        public string StreetName
        {
            get { return _streetName; }
        }

        public int HouseNumber
        {
            get { return _houseNumber; }
        }
    }
}
