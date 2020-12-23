namespace NRules.Samples.SimpleRules.Domain
{
    public class Account
    {
        public string AccountNumber { get; }
        public Customer Owner { get; }
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