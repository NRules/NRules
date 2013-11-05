using System;
using NRules.Fluent.Dsl;

namespace SimpleRulesTest
{
    public class MatchPolicyRule : Rule
    {
        public override void Define()
        {
            Policy policy = null;
            Dwelling dwelling = null;
            Customer customer = null;

            When()
                .If<Policy>(() => policy, x => x.PolicyType == PolicyTypes.Home)
                .If<Customer>(() => customer, x => x.Age > 20, x => x.Policy == policy)
                .If<Dwelling>(() => dwelling, x => x.Type == DwellingTypes.SingleHouse, x => policy.Dwelling == x);

            Then()
                .Do(ctx => Console.WriteLine("Policy={0}, Customer={1} from {2}",
                                             policy.Name, customer.Name, dwelling.Address));
        }
    }
}