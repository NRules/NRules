using System;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class DicsountNotificationRule : Rule
    {
        public override void Define()
        {
            Customer customer = default;

            When()
                .Match(() => customer)
                .Exists<Order>(o => o.Customer == customer, o => o.PercentDiscount > 0.0);

            Then()
                .Do(_ => Console.WriteLine("Customer {0} was notified about a discount", customer.Name));
        }
    }
}