using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class LargeTotalRule : Rule
    {
        public override void Define()
        {
            Customer customer = null;
            IEnumerable<Order> orders = null;
            double total = 0;

            When()
                .Match<Customer>(() => customer, c => c.IsPreferred)
                .Query(() => orders, x => x
                    .Match<Order>(
                        o => o.Customer == customer,
                        o => o.IsOpen)
                    .Collect())
                .Calculate(() => total, () => orders.Sum(x => x.Amount))
                .Having(() => total > 100);

            Then()
                .Do(ctx => Console.WriteLine("Customer {0} has orders over $100 total", customer.Name));
        }
    }
}