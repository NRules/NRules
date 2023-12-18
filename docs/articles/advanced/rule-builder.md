# Creating Rules at Runtime with the RuleBuilder

As described on the [Architecture](../architecture.md) page, rules are represented in NRules in several different forms.
One of these forms is a canonical model (rule model) that can be compiled into the executable model.

The standard way to create rules in NRules is with the internal DSL using fluent API, which requires rules to be known at compile time.
But this is not the only way rules can be created. Rule builder is a component specifically designed to create rules at runtime, and in fact is the underlying mechanism behind fluent API.
[RuleBuilder](xref:NRules.RuleModel.Builders.RuleBuilder) class and related classes reside in the [NRules.RuleModel](xref:NRules.RuleModel) namespace.

> :warning: Unlike fluent API, canonical model and rule builder are not strongly typed, so you will need to ensure type safety and correctness yourself (or face runtime errors if you don't).

When building rules at runtime, you will need a place to store them.
Such a store is represented by the [IRuleRepository](xref:NRules.RuleModel.IRuleRepository) interface, so you will need to implement it to load rules from a user-defined source.
A rule repository is an in-memory database of rule definitions, organized into rule sets ([IRuleSet](xref:NRules.RuleModel.IRuleSet)), that encapsulates the process of turning rules in whatever form to the canonical model.
```c#
public class CustomRuleRepository : IRuleRepository
{
    private readonly IRuleSet _ruleSet = new RuleSet("MyRuleSet");

    public IEnumerable<IRuleSet> GetRuleSets()
    {
        return new[] {_ruleSet};
    }

    public void LoadRules()
    {
        //Assuming there is only one rule in this example
        var rule = BuildRule();
        _ruleSet.Add(new []{rule});
    }

    private IRuleDefinition BuildRule()
    {
        //...
    }
}
```

We will use a simple contrived domain model for our custom rule.
```c#
public class Customer
{
    public Customer(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}

public class Order
{
    public Order(Customer customer, decimal amount)
    {
        Customer = customer;
        Amount = amount;
    }

    public Customer Customer { get; private set; }
    public decimal Amount { get; private set; }
}
```

Now let's implement that `CustomRuleRepository.BuildRule` method. We will create the following rule:
> **Name** TestRule
> 
> **When**
> - Customer name is John Do
> - And this customer has an order in the amount > $100
> 
> **Then**
> - Print customer's name and order amount

Here is the code
```c#
private IRuleDefinition BuildRule()
{
    //Create rule builder
    var builder = new RuleBuilder();
    builder.Name("TestRule");

    //Build conditions
    PatternBuilder customerPattern = builder.LeftHandSide().Pattern(typeof (Customer), "customer");
    Expression<Func<Customer, bool>> customerCondition = 
        customer => customer.Name == "John Do";
    customerPattern.Condition(customerCondition);

    PatternBuilder orderPattern = builder.LeftHandSide().Pattern(typeof (Order), "order");
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
```

A few notes about rule builder code.
- A condition expression may only reference parameters that correspond to patterns that have been previously defined (using [Pattern](xref:NRules.RuleModel.Builders.GroupBuilder.Pattern(System.Type,System.String)) method).
- Names and types of the lambda expression parameters matter and must match the names and types defined in the patterns.
- The first argument of action expression must be of type [IContext](xref:NRules.RuleModel.IContext). You can use [IContext](xref:NRules.RuleModel.IContext) to interact with the engine (i.e. insert new facts).
- Lambda expressions don't have to be defined at compile time. Use various static methods on the BCL's [Expression](xref:System.Linq.Expressions.Expression) class to compose expression trees at runtime.

Putting this all together, here is the test code.
```c#
var repository = new CustomRuleRepository();
repository.LoadRules();
ISessionFactory factory = repository.Compile();

ISession session = factory.CreateSession();
var customer = new Customer("John Do");
session.Insert(customer);
session.Insert(new Order(customer, 90.00m));
session.Insert(new Order(customer, 110.00m));

session.Fire();
```