using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class OrdersByCustomerRule : Rule
    {
        public override void Define()
        {
            IGrouping<Customer, Order> group = null;

            When()
                .Query(() => group, q => 
                    from o in q.Match<Order>()
                    where o.IsOpen
                    group o by o.Customer into g 
                    select g);

            Then()
                .Do(ctx => Console.WriteLine("Customer {0} has {1} open order(s)", 
                    group.Key.Name, group.Count()));
        }
    }
}