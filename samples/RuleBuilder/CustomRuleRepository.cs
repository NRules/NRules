using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Samples.RuleBuilder.Domain;

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
            _ruleSet.Add(new []
            {
                BuildJohnDoLargeOrderRule(),
                BuildMultipleOrdersRule(),
                BuildImportantCustomerRule(),
            });
        }

        private IRuleDefinition BuildJohnDoLargeOrderRule()
        {
            //rule "John Do Large Order Rule"
            //when
            //    customer = Customer(x => x.Name == "John Do");
            //    order = Order(x => x.Customer == customer, x => x.Amount > 100);
            //then
            //    Console.WriteLine("Customer {0} has an order in amount of ${1}", customer.Name, order.Amount);

            var builder = new RuleModel.Builders.RuleBuilder();
            builder.Name("John Do Large Order Rule");

            PatternBuilder customerPattern = builder.LeftHandSide().Pattern(typeof (Customer), "customer");
            ParameterExpression customerParameter = customerPattern.Declaration.ToParameterExpression();
            var customerCondition = Expression.Lambda(
                Expression.Equal(
                    Expression.Property(customerParameter, "Name"),
                    Expression.Constant("John Do")),
                customerParameter);
            customerPattern.Condition(customerCondition);

            PatternBuilder orderPattern = builder.LeftHandSide().Pattern(typeof (Order), "order");
            Expression<Func<Order, Customer, bool>> orderCondition1 = (order, customer) => order.Customer == customer;
            orderPattern.Condition(orderCondition1);
            Expression<Func<Order, bool>> orderCondition2 = order => order.Amount > 100.00;
            orderPattern.Condition(orderCondition2);

            Expression<Action<IContext, Customer, Order>> action = 
                (ctx, customer, order) => Console.WriteLine("Customer {0} has an order in amount of ${1}", customer.Name, order.Amount);
            builder.RightHandSide().Action(action);

            return builder.Build();
        }

        private IRuleDefinition BuildMultipleOrdersRule()
        {
            //rule "Multiple Orders Rule"
            //when
            //    customer = Customer(x => x.IsPreferred);
            //    orders = Query(
            //        Order(x => x.Customer == customer, x => x.IsOpen)
            //        Collect()
            //        Where(c => c.Count() >= 3)
            //    );
            //then
            //    Console.WriteLine("Customer {0} has {1} open order(s)", customer.Name, orders.Count());

            var builder = new RuleModel.Builders.RuleBuilder();
            builder.Name("Multiple Orders Rule");

            PatternBuilder customerPattern = builder.LeftHandSide().Pattern(typeof(Customer), "customer");
            Expression<Func<Customer, bool>> customerCondition = customer => customer.IsPreferred;
            customerPattern.Condition(customerCondition);

            PatternBuilder ordersPattern = builder.LeftHandSide().Pattern(typeof(IEnumerable<Order>), "orders");
            Expression<Func<IEnumerable<Order>, bool>> aggregateCondition = orders => orders.Count() >= 3;
            ordersPattern.Condition(aggregateCondition);

            var aggregate = ordersPattern.Aggregate();
            aggregate.Collect();

            var orderPattern = aggregate.Pattern(typeof(Order), "order");
            Expression<Func<Order, Customer, bool>> orderCondition1 = (order, customer) => order.Customer == customer;
            orderPattern.Condition(orderCondition1);
            Expression<Func<Order, bool>> orderCondition2 = order => order.IsOpen;
            orderPattern.Condition(orderCondition2);

            Expression<Action<IContext, Customer, IEnumerable<Order>>> action =
                (ctx, customer, orders) => Console.WriteLine("Customer {0} has {1} open order(s)", customer.Name, orders.Count());
            builder.RightHandSide().Action(action);

            return builder.Build();
        }

        private IRuleDefinition BuildImportantCustomerRule()
        {
            //rule "Important Customer Rule"
            //when
            //    customer = Customer(x => x.IsPreferred);
            //    or
            //    (
            //        customer = Customer(x => !x.IsPreferred)
            //        exists Order(o => o.Customer == customer, o => o.Amount >= 1000.00)
            //    )
            //then
            //    Console.WriteLine("Customer {0} is important", customer.Name);

            var builder = new RuleModel.Builders.RuleBuilder();
            builder.Name("Important Customer Rule");

            var orGroup = builder.LeftHandSide().Group(GroupType.Or);

            PatternBuilder customerPattern1 = orGroup.Pattern(typeof(Customer), "customer");
            Expression<Func<Customer, bool>> customerCondition1 = customer => customer.IsPreferred;
            customerPattern1.Condition(customerCondition1);

            var andGroup = orGroup.Group(GroupType.And);

            PatternBuilder customerPattern2 = andGroup.Pattern(typeof(Customer), "customer");
            Expression<Func<Customer, bool>> customerCondition2 = customer => !customer.IsPreferred;
            customerPattern2.Condition(customerCondition2);

            PatternBuilder orderExistsPattern = andGroup.Exists().Pattern(typeof(Order), "order");
            Expression<Func<Order, bool>> orderCondition = order => order.Amount >= 1000;
            orderExistsPattern.Condition(orderCondition);

            Expression<Action<IContext, Customer>> action =
                (ctx, customer) => Console.WriteLine("Customer {0} is important", customer.Name);
            builder.RightHandSide().Action(action);

            return builder.Build();
        }
    }
}