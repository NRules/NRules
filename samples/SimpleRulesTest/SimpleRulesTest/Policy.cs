namespace SimpleRulesTest
{
    public enum PolicyTypes
    {
        Auto = 0,
        Home = 1,
    }

    public class Policy
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public PolicyTypes PolicyType { get; set; }
        public Dwelling Dwelling { get; set; }
    }
}