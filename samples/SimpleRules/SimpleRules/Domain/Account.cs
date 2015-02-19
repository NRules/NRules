namespace NRules.Samples.SimpleRules.Domain
{
    public class Account
    {
        public string AccountNumber { get; private set; }
        public Customer Owner { get; private set; }
        public bool IsActive { get; set; }
        public bool IsDelinquent { get; set; }

        public Account(string accountNumber, Customer owner)
        {
            AccountNumber = accountNumber;
            Owner = owner;
            IsActive = true;
            IsDelinquent = false;
        }
    }
}