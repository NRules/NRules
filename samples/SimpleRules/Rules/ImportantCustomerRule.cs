using System;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class ImportantCustomerRule : Rule
    {
        public override void Define()
        {
            Customer customer = default;

            When()
            .Or(x => x
                .Match(() => customer, c => c.IsPreferred)
                .And(xx => xx
                    .Match(() => customer, c => !c.IsPreferred)
                    .Exists<Order>(o => o.Customer == customer, o => o.Amount >= 1000.00)));

            Then()
                .Do(ctx => Console.WriteLine("Customer {0} is important", customer.Name));
        }
    }
}