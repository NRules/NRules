namespace NRules.Samples.RuleBuilder.Domain
{
    public class Customer
    {
        public string Name { get; private set; }
        public bool IsPreferred { get; set; }

        public Customer(string name)
        {
            Name = name;
        }
    }
}