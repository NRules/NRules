using System;
using NRules.Fluent.Dsl;
using NRules.Samples.SimpleRules.Domain;

namespace NRules.Samples.SimpleRules.Rules
{
    public class AllActiveAccountsRule : Rule
    {
        public override void Define()
        {
            Customer customer = default;

            When()
                .Match(() => customer, c => c.IsPreferred)
                .All<Account>(a => a.Owner == customer && a.IsActive, a => !a.IsDelinquent);

            Then()
                .Do(ctx => Console.WriteLine("All active accounts of customer {0} are not delinquent", customer.Name));
        }
    }
}