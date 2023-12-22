# Forward Chaining

A rule in NRules consists of a set of conditions and a set of actions. When a rule matches a set of facts (facts satisfy all rule's conditions), the rule is activated. If it's the highest priority rule to get activated, the rule fires, and its actions are executed.
The rules are not limited to only matching facts that were inserted into the engine's memory from the outside of the engine. Rules themselves can produce new facts (or change existing ones), which then would cause other rules to fire. This process of one rule causing the firing of another rule is called forward chaining.

Forward chaining is useful in decomposition of the business logic, where complex pieces of logic are broken up into orthogonal rules, and a forward-chained fact serves as an interface between them. Examples of this approach include calculation rules that encapsulate a calculation of a business concept or a new metric on an existing fact, and then other rules using that concept/metric without being concerned about how it was produced.

Rules can interact with the rules engine from within the actions using the [IContext](xref:NRules.RuleModel.IContext) parameter passed to them by the engine. In particular, rule's actions can insert new facts and update or retract existing ones.
When a rule creates new facts, there are two ways in which it can do it - inserting a standalone fact or inserting a linked fact.

When a rule inserts a new standalone fact, the fact continues to exist in the engine until it is explicitly retracted, even if the conditions for the rule that produced it become false. These facts are inserted into the rules engine using the [Insert](xref:NRules.RuleModel.IContext.Insert(System.Object)), [InsertAll](xref:NRules.RuleModel.IContext.InsertAll(System.Collections.Generic.IEnumerable{System.Object})) or [TryInsert](xref:NRules.RuleModel.IContext.TryInsert(System.Object)) methods.

The rule can also create a linked fact, which only remains in the engine's memory for as long as the rule that produced it still holds true. If the rule that created a linked fact is no longer active, the linked facts are automatically retracted. Linked facts are inserted into the rules engine using the [InsertLinked](xref:NRules.RuleModel.IContext.InsertLinked(System.Object,System.Object)) method.
While managing standalone forward-chained facts can be done by directly calling methods on the [IContext](xref:NRules.RuleModel.IContext), managing linked facts is far easier using fluent DSL approach.

## Fluent Forward Chaining
NRules Fluent DSL simplifies creation of linked facts to enable forward chaining. It also automatically keeps linked facts in sync with the rules that produced them.
Instead of using a [Do](xref:NRules.Fluent.Dsl.IRightHandSideExpression.Do(System.Linq.Expressions.Expression{System.Action{NRules.RuleModel.IContext}})) action, a rule can use [Yield](xref:NRules.Fluent.Dsl.IRightHandSideExpression.Yield``1(System.Linq.Expressions.Expression{System.Func{NRules.RuleModel.IContext,``0}})) to generate a linked fact. If this is the first activation of the rule for a given set of matching facts, the new linked fact is created. If this is a subsequent activation of the rule (i.e. due to an update to the matching facts), the corresponding linked fact is also updated. Finally, if the rule no longer matches, the corresponding linked facts are automatically retracted.
There are two overloads of the `Yield` method - one with the same calculation for inserts and updates, and another one with different calculations - in the later case an update has access to the previous value of the linked fact.

```c#
public class PreferredCustomerDiscountRule : Rule
{
    public override void Define()
    {
        Customer customer = null;
        IEnumerable<Order> orders = null;
        Double total = Double.NaN;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Query(() => orders, x => x
                .Match<Order>(
                    o => o.Customer == customer,
                    o => o.IsOpen)
                .Collect())
            .Let(() => total, () => orders.Sum(x => x.Amount))
            .Having(() => total > 1000);

        Then()
            .Yield(_ => new InstantDiscount(customer, total * 0.05));
    }
}

public class PrintInstantDiscountRule : Rule
{
    public override void Define()
    {
        InstantDiscount discount = null;

        When()
            .Match(() => discount);

        Then()
            .Do(_ => Console.WriteLine("Customer {0} has instant discount of {1}", 
                discount.Customer.Name, discount.Amount));
    }
}
```
