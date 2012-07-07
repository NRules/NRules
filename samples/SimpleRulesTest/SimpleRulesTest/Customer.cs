namespace SimpleRulesTest
{
    public enum SexTypes
    {
        Male = 0,
        Female = 1,
    }

    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public SexTypes Sex { get; set; }
        public Policy Policy { get; set; }
    }
}