using NRules;

namespace RuleBuilder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var repository = new CustomRuleRepository();
            repository.LoadRules();

            ISessionFactory factory = repository.Compile();
            ISession session = factory.CreateSession();

            var customer1 = new Customer("John Do");
            var customer2 = new Customer("Jean Do");
            session.Insert(customer1);
            session.Insert(customer2);
            session.Insert(new Order(customer1, 90.00m));
            session.Insert(new Order(customer1, 110.00m));
            session.Insert(new Order(customer1, 1000.00m));
            session.Insert(new Order(customer2, 120.00m));

            session.Fire();
        }
    }
}