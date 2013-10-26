using NRules.Inline;

namespace SimpleRulesTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dwelling = new Dwelling() {Address = "1 Main Street, New York, NY", Type = DwellingTypes.SingleHouse};
            var dwelling2 = new Dwelling() {Address = "2 Main Street, New York, NY", Type = DwellingTypes.SingleHouse};
            var policy1 = new Policy() {Name = "Silver", PolicyType = PolicyTypes.Home, Price = 1200, Dwelling = dwelling};
            var policy2 = new Policy() {Name = "Gold", PolicyType = PolicyTypes.Home, Price = 2300, Dwelling = dwelling2};
            var customer1 = new Customer() {Name = "John Do", Age = 22, Sex = SexTypes.Male, Policy = policy1};
            var customer2 = new Customer() {Name = "Emily Brown", Age = 32, Sex = SexTypes.Female, Policy = policy2};

            IInlineRepository repository = new InlineRepository();
            repository.AddFromAssembly(typeof (Program).Assembly);

            var factory = repository.CreateSessionFactory();
            var session = factory.CreateSession();

            session.Insert(policy1);
            session.Insert(policy2);
            session.Insert(customer1);
            session.Insert(customer2);
            session.Insert(dwelling);
            session.Insert(dwelling2);

            customer1.Age = 10;
            session.Update(customer1);

            session.Retract(customer2);

            session.Fire();

            session.Insert(customer2);

            session.Fire();

            customer1.Age = 30;
            session.Update(customer1);

            session.Fire();
        }
    }
}