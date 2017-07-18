namespace NRules.Samples.SimpleRules.Domain
{
    public class Order
    {
        public int Id { get; }
        public Customer Customer { get; }
        public int Quantity { get; }
        public double UnitPrice { get; }
        public double PercentDiscount { get; private set; }
        public bool IsOpen { get; }
        public bool IsDiscounted => PercentDiscount > 0;
        public double Amount => UnitPrice*Quantity*(1.0 - PercentDiscount/100.0);

        public Order(int id, Customer customer, int quantity, double unitPrice)
        {
            Id = id;
            Customer = customer;
            Quantity = quantity;
            UnitPrice = unitPrice;
            IsOpen = true;
        }

        public void ApplyDiscount(double percentDiscount)
        {
            PercentDiscount = percentDiscount;
        }
    }
}