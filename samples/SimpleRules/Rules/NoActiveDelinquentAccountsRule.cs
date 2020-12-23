using System;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class NoActiveDelinquentAccountsRule : Rule
    {
        public override void Define()
        {
            Customer customer = default;

            When()
                .Match(() => customer, c => c.IsPreferred)
                .Not<Account>(a => a.Owner == customer, a => a.IsActive, a => a.IsDelinquent);

            Then()
                .Do(ctx => Console.WriteLine("Customer {0} does not have active delinquent accounts", customer.Name));
        }
    }
}