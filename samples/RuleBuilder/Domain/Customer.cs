namespace NRules.Samples.RuleBuilder.Domain;

public class Customer
{
    public string Name { get; }
    public bool IsPreferred { get; set; }

    public Customer(string name)
    {
        Name = name;
    }
}