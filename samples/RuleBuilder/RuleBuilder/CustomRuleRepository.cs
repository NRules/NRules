using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Samples.RuleBuilder
{
    public class CustomRuleRepository : IRuleRepository
    {
        private readonly IRuleSet _ruleSet = new RuleSet("DefaultRuleSet");

        public IEnumerable<IRuleSet> GetRuleSets()
        {
            return new[] {_ruleSet};
        }

        public void LoadRules()
        {
            var rule = BuildRule();
            _ruleSet.Add(new []{rule});
        }

        private IRuleDefinition BuildRule()
        {
            //Create rule builder
            var builder = new NRules.RuleModel.Builders.RuleBuilder();
            builder.Name("TestRule");

            //Build conditions
            PatternBuilder customerPattern = builder.LeftHandSide().Pattern(typeof (Customer), "customer");
            //Can compose expressions at runtime
            ParameterExpression customerParameter = customerPattern.Declaration.ToParameterExpression();
            var customerCondition = Expression.Lambda(
                Expression.Equal(
                    Expression.Property(customerParameter, "Name"),
                    Expression.Constant("John Do")),
                customerParameter
                );
            customerPattern.Condition(customerCondition);

            PatternBuilder orderPattern = builder.LeftHandSide().Pattern(typeof (Order), "order");
            //Can specify expression at compile time
            Expression<Func<Order, Customer, bool>> orderCondition1 = 
                (order, customer) => order.Customer == customer;
            Expression<Func<Order, bool>> orderCondition2 = 
                order => order.Amount > 100.00m;
            orderPattern.Condition(orderCondition1);
            orderPattern.Condition(orderCondition2);

            //Build actions
            Expression<Action<IContext, Customer, Order>> action = 
                (ctx, customer, order) => Console.WriteLine("Customer {0} has an order in amount of ${1}", customer.Name, order.Amount);
            builder.RightHandSide().Action(action);

            //Build rule model
            return builder.Build();
        }
    }
}