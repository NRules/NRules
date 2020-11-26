using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    [Name("Preferred customer discount")]
    public class PreferredCustomerDiscountRule : Rule
    {
        public override void Define()
        {
            Customer customer = null;
            IEnumerable<Order> orders = null;

            When()
                .Match<Customer>(() => customer, c => c.IsPreferred)
                .Query(() => orders, x => x
                    .Match<Order>(
                        o => o.Customer == customer,
                        o => o.IsOpen,
                        o => !o.IsDiscounted)
                    .Collect()
                    .Where(c => c.Any()));

            Then()
                .Do(ctx => ApplyDiscount(orders, 10.0))
                .Do(ctx => LogOrders(orders))
                .Do(ctx => orders.ToList().ForEach(ctx.Update));
        }

        private static void ApplyDiscount(IEnumerable<Order> orders, double discount)
        {
            foreach (var order in orders)
            {
                order.ApplyDiscount(discount);
            }
        }

        private static void LogOrders(IEnumerable<Order> orders)
        {
            Console.WriteLine("Discount applied to orders {0}", 
                string.Join(",", orders.Select(o => o.Id)));
        }
    }
}