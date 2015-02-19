using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class MultipleOrdersRule : Rule
    {
        public override void Define()
        {
            Customer customer = null;
            IEnumerable<Order> orders = null;

            When()
                .Match<Customer>(() => customer, c => c.IsPreferred)
                .Collect<Order>(() => orders, o => o.Customer == customer, o => o.IsOpen)
                    .Where(x => x.Count() >= 3);

            Then()
                .Do(ctx => Console.WriteLine("Customer {0} has over 3 open orders", customer.Name));
        }
    }
}