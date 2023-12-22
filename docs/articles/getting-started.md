# Getting Started

This guide shows step by step how to install and use NRules rules engine.

## Installing NRules
Create a new Visual studio Console App project and install NRules

# [CLI](#tab/cli)
```console
> dotnet add package NRules
```
# [Package Manager](#tab/pm)
```console
> Install-Package NRules
```
---

If you have rules and rules engine runtime in different projects, then instead of depending on the `NRules` package (which is a meta-package), you can fine-tune the dependencies. The rules project should depend on the `NRules.Fluent` package, while the runtime components should depend on `NRules.Runtime`.

## Creating Domain Model
NRules is geared towards writing rules against a domain model, so we start by creating a simple one, which describes customers and orders.

```c#
public class Customer
{
    public string Name { get; }
    public bool IsPreferred { get; set; }

    public Customer(string name)
    {
        Name = name;
    }

    public void NotifyAboutDiscount()
    {
        Console.WriteLine($"Customer {Name} was notified about a discount");
    }
}

public class Order
{
    public int Id { get; }
    public Customer Customer { get; }
    public int Quantity { get; }
    public double UnitPrice { get; }
    public double PercentDiscount { get; set; }
    public bool IsOpen { get; set; } = true;

    public Order(int id, Customer customer, int quantity, double unitPrice)
    {
        Id = id;
        Customer = customer;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
```

## Creating Rules
When using NRules internal DSL, a rule is a class that inherits from [Rule](xref:NRules.Fluent.Dsl.Rule). A rule consists of a set of conditions (patterns that match facts in the rules engine's memory) and a set of actions executed by the engine should the rule fire.

Let's look at the first rule. We want to find all preferred customers, and for every matching customer we want to collect all orders and apply a discount of 10%.
Each pattern in the [When](xref:NRules.Fluent.Dsl.Rule.When) part of the rule is bound to a variable via an expression, and then can be used in the [Then](xref:NRules.Fluent.Dsl.Rule.Then) part of the rule. Also note that if there is more than one pattern in the rule, the patterns must be joined to avoid a Cartesian Product between the matching facts. In this example the orders are joined with the customer.
```c#
public class PreferredCustomerDiscountRule : Rule
{
    public override void Define()
    {
        Customer customer = default;
        IEnumerable<Order> orders = default;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Query(() => orders, x => x
                .Match<Order>(
                    o => o.Customer == customer,
                    o => o.IsOpen,
                    o => o.PercentDiscount == 0.0)
                .Collect()
                .Where(c => c.Any()));

        Then()
            .Do(ctx => ApplyDiscount(orders, 10.0))
            .Do(ctx => ctx.UpdateAll(orders));
    }

    private static void ApplyDiscount(IEnumerable<Order> orders, double discount)
    {
        foreach (var order in orders)
        {
            order.PercentDiscount = discount;
        }
    }
}
```

The second rule will find all customers that have orders with discounts and will notify them of the discount. It's interesting that this rule relies on the first rule to have fired. In other words, the first rule fires and updates the rules engine's memory, triggering the second rule. This is forward chaining in action.
```c#
public class DiscountNotificationRule : Rule
{
    public override void Define()
    {
        Customer customer = default;

        When()
            .Match<Customer>(() => customer)
            .Exists<Order>(o => o.Customer == customer, o => o.PercentDiscount > 0.0);

        Then()
            .Do(_ => customer.NotifyAboutDiscount());
    }
}
```

## Running Rules
NRules is an inference engine. It means there is no predefined order in which rules are executed, and it runs a match/resolve/act cycle to figure it out. It first matches facts (instances of domain entities) with the rules and determines which rules can fire. The rules that matched facts are said to be activated. It then resolves the conflict by choosing a single rule that will actually fire. And, finally, it fires the chosen rule by executing its actions. The cycle is repeated until there are no more rules to fire.
We need to do several things for the engine to enter the match/resolve/act cycle.
First, we need to load the rules and compile them into an internal structure (Rete network), so that the engine knows what the rules are and can efficiently match facts. We do this by creating a rule repository and letting it scan an assembly to find the rule classes. Then we compile the rules into a session factory.
Next we need to create a working session with the engine and insert facts into the engine's memory.
Finally we tell the engine to start the match/resolve/act cycle.

```c#
//Load rules
var repository = new RuleRepository();
repository.Load(x => x.From(typeof(PreferredCustomerDiscountRule).Assembly));

//Compile rules
var factory = repository.Compile();

//Create a working session
var session = factory.CreateSession();

//Load domain model
var customer = new Customer("John Doe") {IsPreferred = true};
var order1 = new Order(123456, customer, 2, 25.0);
var order2 = new Order(123457, customer, 1, 100.0);

//Insert facts into rules engine's memory
session.Insert(customer);
session.Insert(order1);
session.Insert(order2);

//Start match/resolve/act cycle
session.Fire();
```

This prints
```console
Customer John Doe was notified about a discount
```
