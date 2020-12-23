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
            Customer customer = default;
            IEnumerable<Order> orders = default;
            double total = default;

            When()
                .Match(() => customer, c => c.IsPreferred)
                .Query(() => orders, x => x
                    .Match<Order>(
                        o => o.Customer == customer,
                        o => o.IsOpen)
                    .Collect())
                .Let(() => total, () => orders.Sum(x => x.Amount))
                .Having(() => total > 100);

            Then()
                .Do(ctx => Console.WriteLine("Customer {0} has orders over $100 total", customer.Name));
        }
    }
}