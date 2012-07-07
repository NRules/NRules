using System;
using NRules;
using NRules.Dsl;

namespace SimpleRulesTest
{
    public class MatchPolicyRule : IRule
    {
        public void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<Policy>(policy => policy.PolicyType == PolicyTypes.Home)
                .If<Dwelling>(dwelling => dwelling.Type == DwellingTypes.SingleHouse)
                .If<Customer>(customer => customer.Age > 20)
                .If<Customer, Policy>((customer, policy) => customer.Policy == policy)
                .If<Dwelling, Policy>((dwelling, policy) => policy.Dwelling == dwelling);

            definition.Then()
                .Do(ctx => Console.WriteLine("Policy={0}, Customer={1} from {2}",
                                             ctx.Arg<Policy>().Name, ctx.Arg<Customer>().Name, ctx.Arg<Dwelling>().Address));
        }
    }
}