using System.Reflection;
using NRules.Fluent;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Load rules
            var repository = new RuleRepository();
            repository.Load(x => x.From(Assembly.GetExecutingAssembly()));

            //Compile rules
            var factory = repository.Compile();

            //Create a working session
            var session = factory.CreateSession();

            //Load domain model
            var customer1 = new Customer("John Doe") { IsPreferred = true };
            var account11 = new Account("12456789", customer1);
            var account12 = new Account("12456790", customer1);
            var account13 = new Account("12456791", customer1) {IsActive = false, IsDelinquent = true};
            var order11 = new Order(123456, customer1, 2, 25.0);
            var order12 = new Order(123457, customer1, 1, 100.0);
            var order13 = new Order(123458, customer1, 1, 5.0);

            var customer2 = new Customer("Sarah Jones");
            var account21 = new Account("22456789", customer2);
            var order21 = new Order(223456, customer2, 2, 2225.0);

            //Insert facts into rules engine's memory
            session.Insert(customer1);
            session.Insert(account11);
            session.Insert(account12);
            session.Insert(account13);
            session.Insert(order11);
            session.Insert(order12);
            session.Insert(order13);

            session.Insert(customer2);
            session.Insert(account21);
            session.Insert(order21);

            //Start match/resolve/act cycle
            session.Update(customer1);
            session.Fire();
        }
    }
}