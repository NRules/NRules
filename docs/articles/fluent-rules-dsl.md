# Fluent Rules DSL

The standard way to define rules in NRules is with the internal DSL using fluent C# API.

A rule is a .NET class that inherits from [Rule](xref:NRules.Fluent.Dsl.Rule).
Rule class overrides [Define](xref:NRules.Fluent.Dsl.Rule.Define) method where the actual conditions (left-hand side, or LHS) and actions (right-hand side, or RHS) parts are specified.

Within the [Define](xref:NRules.Fluent.Dsl.Rule.Define) method, LHS is specified by fluently chaining conditions to the [When()](xref:NRules.Fluent.Dsl.Rule.When) method; and RHS by fluently chaining actions to [Then()](xref:NRules.Fluent.Dsl.Rule.Then) method.

> [!WARNING]
> Make sure rule classes are public, otherwise the engine won't find them.

See [Fluent Rules Loading](fluent-rules-loading.md) on how to load rules defined with the fluent rules DSL.

Rule class can also optionally be decorated with the following custom attributes to associate additional metadata with the rule.

Attribute | Allow Multiple? | Inherited? | Description
--- | --- | --- | ---
[Name](xref:NRules.Fluent.Dsl.NameAttribute) | No | No | Specifies rule name. Default is the fully qualified rule class name.
[Description](xref:NRules.Fluent.Dsl.DescriptionAttribute) | No | No | Specifies rule description. Default is an empty string.
[Tag](xref:NRules.Fluent.Dsl.TagAttribute) | Yes | Yes | Associates arbitrary tag with the rule (can later be used to group or filter rules).
[Priority](xref:NRules.Fluent.Dsl.PriorityAttribute) | No | Yes | Sets rule priority. If multiple rules get activated at the same time, rules with higher priority (larger number) get executed first. Priority can be positive or negative. Default is zero.
[Repeatability](xref:NRules.Fluent.Dsl.RepeatabilityAttribute) | No | No | Sets rule's repeatability, that is, how it behaves when it is activated with the same set of facts multiple times, which is important for recursion control. Repeatability can be set to repeatable - the rule is activated if any of the matched facts are updated, or non-repeatable - the rule is only activated once for a given set of matched facts (unless the match is retracted, before being re-asserted again). Default is `Repeatable`.

```c#
[Name("MyRule"), Description("Test rule that demonstrates metadata usage")]
[Tag("Test"), Tag("Metadata")]
[Priority(10)]
[Repeatability(RuleRepeatability.Repeatable)]
public class RuleWithMetadata : Rule
{
    public override void Define()
    {
        When()
            .Match<Customer>(c => c.Name == "John Do");
        Then()
            .Do(ctx => DoSomething());
    }
}
```

While fluent rules DSL uses C#, the rules have to be defined using declarative approach. There should be no imperative C# code used anywhere in the rule definition, except within condition expressions, action expressions and methods called from those expressions.

If a rule pattern is bound to a variable (see below), that variable should only be used in subsequent condition and action expressions directly. The purpose of the binding variable is to serve as a token (that has a name and a type) that instructs the engine to link corresponding conditions and actions. Don't write any code that manipulates the binding variables outside of the condition/action expressions.

## Matching Facts with Patterns
Rule's left hand side is a set of patterns that match facts of a given type. A pattern is defined using a [Match](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.Match``1(System.Linq.Expressions.Expression{System.Func{``0}},System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}}[])) method.
A pattern can have zero, one or many conditions that must all be true in order for the pattern to match a given fact.

Pattern matching is also polymorphic, which means it matches all facts of a given type and any derived type. Given a class hierarchy of Fruit, Apple and Pear, ```Match<Fruit>``` will match both Apples and Pears. Consequently, ```Match<object>``` will match all facts in the engine's working memory. 

If a given pattern matches multiple facts in the engine’s working memory, each match will result in a separate firing of the rule.

Optionally, a pattern can be bound to a variable, in which case that variable can be used in subsequent patterns to specify inter-fact conditions. Also, the variable can be used inside actions to update or retract the corresponding fact, or use it in the expression. Do not use or otherwise manipulate the binding variable anywhere outside of the condition/action expressions.

```c#
public class PreferredCustomerActiveAccountsRule : Rule
{
    public override void Define()
    {
        Customer customer = default!;
        Account account = default!;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Match<Account>(() => account, a => a.Owner == customer, a => a.IsActive);

        Then()
            .Do(ctx => customer.DoSomething());
    }
}
```

## Existential Rules
Existential rules test for presence of facts that match a particular set of conditions. An existential quantifier is defined using [Exists](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.Exists``1(System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}}[])) method.

An existential quantifier cannot be bound to a variable, since it does not match any single fact.

```c#
public class PreferredCustomerActiveAccountsRule : Rule
{
    public override void Define()
    {
        Customer customer = default!;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Exists<Account>(a => a.Owner == customer, a => a.IsActive);

        Then()
            .Do(ctx => customer.DoSomething());
    }
}
```

## Negative Rules
Opposite to existential rules, negative rules test for absence of facts that match a particular set of conditions. A negative existential quantifier is defined using [Not](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.Not``1(System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}}[])) method.

A negative existential quantifier cannot be bound to a variable, since it does not match any single fact.

```c#
public class PreferredCustomerNotDelinquentRule : Rule
{
    public override void Define()
    {
        Customer customer = default!;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Not<Account>(a => a.Owner == customer, a => a.IsDelinquent);

        Then()
            .Do(ctx => customer.DoSomething());
    }
}
```

## Universal Quantifier
Universal quantifier ensures that all facts that match a particular condition also match all subsequent conditions defined by the quantifier. A universal quantifier is defined using [All](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.All``1(System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}},System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}}[])) method.

A universal quantifier cannot be bound to a variable, since it does not match any single fact.

```c#
public class PreferredCustomerAllAccountsActiveRule : Rule
{
    public override void Define()
    {
        Customer customer = default!;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .All<Account>(a => a.Owner == customer, a => a.IsActive);

        Then()
            .Do(ctx => customer.DoSomething());
    }
}
```

## Grouping Patterns
By default all patterns on the left-hand side of the rule are connected using AND operator. This means that all patterns must match for the rule to activate.

Patterns can also be connected using OR operator, as well as combined into nested groups.

```c#
public class PreferredCustomerOrHasLargeOrderRule : Rule
{
    public override void Define()
    {
        Customer customer = default!;

        When()
            .Or(x => x
                .Match<Customer>(() => customer, c => c.IsPreferred)
                .And(xx => xx
                    .Match<Customer>(() => customer, c => !c.IsPreferred)
                    .Exists<Order>(o => o.Customer == customer, o => o.Price >= 1000.00)));

        Then()
            .Do(ctx => customer.DoSomething());
    }
}
```

## Rules with Complex Logic
In complex rules it is usually required to aggregate or project facts, calculate derived values and correlate different matched facts. The rules engine provides several different DSL operators to express such logic.

Rules can match and transform sets of facts using [Query](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.Query``1(System.Linq.Expressions.Expression{System.Func{``0}},System.Func{NRules.Fluent.Dsl.IQuery,NRules.Fluent.Dsl.IQuery{``0}})) syntax, which enables rules authors to apply LINQ operators to matched facts; see [[Reactive LINQ Queries]] for more details.

A [Let](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.Let``1(System.Linq.Expressions.Expression{System.Func{``0}},System.Linq.Expressions.Expression{System.Func{``0}})) operator binds an expression to a variable, so that the results of intermediate calculations can be used in subsequent rule conditions and actions.

Also, [Having](xref:NRules.Fluent.Dsl.ILeftHandSideExpression.Having(System.Linq.Expressions.Expression{System.Func{System.Boolean}}[])) operator can add new conditions to previously defined patterns, including `Let` expressions, improving rules expressiveness and composability.

```c#
public class LargeTotalRule : Rule
{
    public override void Define()
    {
        Customer customer = default!;
        IEnumerable<Order> orders = default!;
        double total = 0;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Query(() => orders, x => x
                .Match<Order>(
                    o => o.Customer == customer,
                    o => o.IsOpen)
                .Collect())
            .Let(() => total, () => orders.Sum(x => x.Amount))
            .Having(() => total > 100);

        Then()
            .Do(ctx => customer.DoSomething(total));
    }
}
```
