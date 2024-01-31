using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Samples.JsonRules.Domain;

namespace NRules.Samples.JsonRules;

internal class Program
{
    private static void Main()
    {
        //Create a rule in code
        var rule = BuildRule();

        //Set up JSON serialization options
        //This bootstraps custom JSON converters for NRules types
        var options = SetupJsonSerializer();

        //Serialize rule to JSON
        var json = JsonSerializer.Serialize(rule, options);
        File.WriteAllText("rule.json", json);

        //Deserialize rule from JSON
        var ruleClone = JsonSerializer.Deserialize<IRuleDefinition>(json, options)!;
        
        //Compile rule
        var compiler = new RuleCompiler();
        var factory = compiler.Compile(new[] { ruleClone });
        var session = factory.CreateSession();
        
        //Test rule
        var customer = new Customer("John Do");
        session.Insert(customer);
        var order = new Order(1, customer, 2, 200.00);
        session.Insert(order);
        
        session.Fire();
    }

    private static JsonSerializerOptions SetupJsonSerializer()
    {
        //Set up type resolver and type aliases
        var typeResolver = new TypeResolver();
        typeResolver.RegisterDefaultAliases();
        typeResolver.RegisterAlias("Customer", typeof(Customer));
        typeResolver.RegisterAlias("Order", typeof(Order));
        typeResolver.RegisterAlias("Context", typeof(IContext));
        typeResolver.RegisterAlias("Console", typeof(Console));

        //Set up JSON serialization options
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        RuleSerializer.Setup(options, typeResolver);
        return options;
    }

    private static IRuleDefinition BuildRule()
    {
        //rule "John Do Large Order Rule"
        //when
        //    customer = Customer(x => x.Name == "John Do");
        //    order = Order(x => x.Customer == customer, x => x.Amount > 100);
        //then
        //    Console.WriteLine("Found large order");

        var builder = new RuleBuilder();
        builder.Name("John Do Large Order Rule");

        PatternBuilder customerPattern = builder.LeftHandSide().Pattern(typeof(Customer), "customer");
        ParameterExpression customerParameter = customerPattern.Declaration.ToParameterExpression();
        var customerCondition = Expression.Lambda(
            Expression.Equal(
                Expression.Property(customerParameter, nameof(Customer.Name)),
                Expression.Constant("John Do")),
            customerParameter);
        customerPattern.Condition(customerCondition);

        PatternBuilder orderPattern = builder.LeftHandSide().Pattern(typeof(Order), "order");
        ParameterExpression orderParameter = orderPattern.Declaration.ToParameterExpression();
        var orderCondition1 = Expression.Lambda(
            Expression.Equal(
                Expression.Property(orderParameter, nameof(Order.Customer)),
                customerParameter),
            orderParameter,
            customerParameter);
        orderPattern.Condition(orderCondition1);
        var orderCondition2 = Expression.Lambda(
            Expression.GreaterThan(
                Expression.Property(orderParameter, nameof(Order.Amount)),
                Expression.Constant(100.00)),
            orderParameter);
        orderPattern.Condition(orderCondition2);

        var ctxParameter = Expression.Parameter(typeof(IContext), "ctx");
        var action = Expression.Lambda(
            Expression.Call(
                typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) })!,
                Expression.Constant("Found large order")),
            ctxParameter
        );
        builder.RightHandSide().Action(action);

        return builder.Build();
    }
}