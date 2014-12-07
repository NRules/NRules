namespace NRules.Samples.RuleBuilder
{
    public class Order
    {
        public Order(Customer customer, decimal amount)
        {
            Customer = customer;
            Amount = amount;
        }

        public Customer Customer { get; private set; }
        public decimal Amount { get; private set; }
    }
}