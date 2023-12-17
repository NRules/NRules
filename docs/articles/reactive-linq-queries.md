# Reactive LINQ Queries

Rules are normally written to match individual facts in the rules engine's working memory. But sometimes it is desirable to match the sets of facts. This is where reactive LINQ queries in NRules come into play.

A query starts by matching all facts that satisfy a particular set of conditions, but instead of firing the rule for each fact match, reactive query allows further aggregation of matching facts using LINQ-style query operators.
For example, one can group matching facts by a set of properties, and can fire the rule for each matching group.

The queries are called reactive, because even though it looks like they are querying the rules engine's memory, they are evaluated incrementally, as facts are inserted, updated or retracted from the rules engine.
Thus, reactive queries are as efficient as matching individual facts.

A query result must be bound to a variable, so that it can then be used within other rule patterns, or by the rule's actions.

The following operators are supported in reactive LINQ queries.

* Match - starts a query by matching facts in rules engine's memory
* Where - filters source elements by a set of conditions
* GroupBy - aggregates source elements into groups
* Collect - aggregates source elements into a collection
* Select - projects source elements
* SelectMany - flattens source collections

```c#
[Name("Preferred customer discount")]
public class PreferredCustomerDiscountRule : Rule
{
    public override void Define()
    {
        Customer customer = null;
        IEnumerable<Order> orders = null;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Query(() => orders, x => x
                .Match<Order>(
                    o => o.Customer == customer,
                    o => o.IsOpen,
                    o => !o.IsDiscounted)
                .Collect()
                .Where(c => c.Any()));

        Then()
            .Do(ctx => ApplyDiscount(orders, 10.0)));
    }
}
```