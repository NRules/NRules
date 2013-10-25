using System;
using NRules.Dsl;

namespace SimpleRulesTest
{
    public class MatchPolicyRule : IRule
    {
        public void Define(IDefinition definition)
        {
            Policy policy = null;
            Dwelling dwelling = null;
            Customer customer = null;

            definition.When()
                .If<Policy>(() => policy, x => x.PolicyType == PolicyTypes.Home)
                .If<Customer>(() => customer, x => x.Age > 20, x => x.Policy == policy)
                .If<Dwelling>(() => dwelling, x => x.Type == DwellingTypes.SingleHouse, x => policy.Dwelling == x);

            definition.Then()
                .Do(() => Console.WriteLine("Policy={0}, Customer={1} from {2}",
                                             policy.Name, customer.Name, dwelling.Address));
        }
    }
}