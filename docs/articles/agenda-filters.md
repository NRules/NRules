# Agenda Filters

When all rule's conditions are satisfied by a set of facts, the rule is said to be activated by those facts. A rule can be activated by a new set of matching facts, or an updated existing set of matching facts.
When a rule is activated, the activation (rule, plus the matching set of facts) is placed on the agenda. Once all activations are calculated, the activation with the highest priority is chosen, the rule fires, and all its actions are executed.

Agenda filters allow applying a set of conditions to activations, before those are placed on the agenda. If an activation passes all the filters, it is placed on the agenda, otherwise it's not. Agenda filters could be a good place to dynamically enable/disable the rules, filter fact updates to activate on a subset of changed values, etc. An already activated rule whose agenda filter evaluates to `false` will not re-fire, but its activation will not be retracted, as is the case when a condition on the left-hand side of the rule evaluates to `false`. This makes agenda filters useful for [forward chaining](forward-chaining.md) rules, since they allow re-firing logic to be distinct from activation/retraction logic.

Agenda filters can be registered in two different ways - as global or as rule-specific filters. A global agenda filter is applied to all activations, while a rule-specific filter is only applied to activations of a particular rule.
Agenda filters only apply to activations that are about to be placed on the agenda, so filters will never remove activations that are already on the agenda.

## Fluent Rule Filters
Rule-specific agenda filters can be added declaratively, using fluent DSL. There are two kinds of filters that can be defined with the fluent DSL - predicate filters that test rule activation against a set of conditions, and change filters that only accept activations where a particular set of keys changed.

Change filters in particular are extremely useful for rule chaining and recursion control. One rule may change one field in the fact and another rule may change a different field in that same fact. Without a change filter, each rule would chain the other one, causing infinite recursion. With change filters both rules can only accept fact changes they care about, thus improving rules composability and eliminating unwanted recursion.

```c#
public class OrderAmountCalculationRule : Rule
{
    public override void Define()
    {
        Order order = default!;

        When()
            .Match(() => order);

        Filter()
            .OnChange(() => order.Quantity, () => order.UnitPrice, () => order.PercentDiscount)
            .Where(() => order.Quantity > 0);

        Then()
            .Do(ctx => ctx.Update(order, CalculateAmount));
    }

    private static void CalculateAmount(Order order)
    {
        order.Amount = order.UnitPrice * order.Quantity * (1.0 - order.PercentDiscount / 100.0);
    }
}
```

This rule will fire the first time a given order is matched, and then subsequently if its Quantity, UnitPrice or PercentDiscount changes ([OnChange](xref:NRules.Fluent.Dsl.IFilterExpression.OnChange(System.Linq.Expressions.Expression{System.Func{System.Object}}[])) filter). This rule will never fire on an order whose Quantity is less than or equal to zero ([Where](xref:NRules.Fluent.Dsl.IFilterExpression.Where(System.Linq.Expressions.Expression{System.Func{System.Boolean}}[])) filter). The ordering of filter conditions doesn't matter.

Multiple `Where` filters are always interpreted to have an `AND` relationship, whereas `OnChange` filters have an `OR`. If both `Where` and `OnChange` filters are defined, the relationship is `(Where1 AND Where2 AND (OnChange1 OR OnChange2))`.

## Defining Rule Filters in Code
An agenda filter can be defined as a class that implements [IAgendaFilter](xref:NRules.AgendaFilters.IAgendaFilter) interface; its [Accept](xref:NRules.AgendaFilters.IAgendaFilter.Accept(NRules.AgendaFilters.AgendaContext,NRules.Activation)) method determines if the activation is to be added to the agenda or not.

Agenda filters are added to the [ISession.Agenda](xref:NRules.ISession.Agenda) using [AddFilter](xref:NRules.IAgenda.AddFilter(NRules.AgendaFilters.IAgendaFilter)) methods. Depending on the specific overload of the method used, the filter is added either as a global or rule-specific filter.

```c#
public class DisabledRuleFilter : IAgendaFilter
{
    public bool Accept(Activation activation)
    {
        if (activation.Rule.Tags.Contains("Disabled")) return false;
        return true;
    }
}

//...

var filter = new DisabledRuleFilter();
var session = factory.CreateSession(x =>
    x.Agenda.AddFilter(filter));
```

## Stateful Agenda Filters
In most cases agenda filters are only concerned with the activation that is getting inserted into the agenda, and so are stateless. But there are cases where it's helpful for an agenda filter to store some state about the activation, so that the next time the same activation is inserted into the agenda, the filter can use that state information. In these cases, instead of implementing [IAgendaFilter](xref:NRules.AgendaFilters.IAgendaFilter) interface, implement [IStatefulAgendaFilter](xref:NRules.AgendaFilters.IStatefulAgendaFilter). This is how `OnChange` filters are implemented.

In addition to the [Accept](xref:NRules.AgendaFilters.IAgendaFilter.Accept(NRules.AgendaFilters.AgendaContext,NRules.Activation)) method, stateful agenda filters are also notified by the engine of various activation lifecycle events, so that the filter can update the state, or remove the state associated with the activation.