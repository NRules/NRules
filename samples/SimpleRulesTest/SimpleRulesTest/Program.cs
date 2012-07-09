using NRules.Core;

namespace SimpleRulesTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dwelling1 = new Dwelling() {Address = "1 Main Street, New York, NY", Type = DwellingTypes.SingleHouse};
            var dwelling2 = new Dwelling() {Address = "2 Main Street, New York, NY", Type = DwellingTypes.SingleHouse};
            var policy1 = new Policy() {Name = "Silver", PolicyType = PolicyTypes.Home, Price = 1200, Dwelling = dwelling1};
            var policy2 = new Policy() {Name = "Gold", PolicyType = PolicyTypes.Home, Price = 2300, Dwelling = dwelling2};
            var customer1 = new Customer() {Name = "John Do", Age = 22, Sex = SexTypes.Male, Policy = policy1};
            var customer2 = new Customer() {Name = "Emily Brown", Age = 32, Sex = SexTypes.Female, Policy = policy2};

            var repository = new RuleRepository();
            repository.AddRuleSet(typeof (Program).Assembly);

            var factory = new SessionFactory(repository);
            var session = factory.CreateSession();

            session.Insert(policy1);
            session.Insert(policy2);
            session.Insert(customer1);
            session.Insert(customer2);
            session.Insert(dwelling1);
            session.Insert(dwelling2);

            session.Retract(customer1);

            session.Fire();
        }
    }
}