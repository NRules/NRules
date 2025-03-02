# DSL Extensions

When defining rules using internal DSL in C#, the DSL is limited to the verbs provided by the rules engine (see [Fluent Rules DSL](fluent-rules-dsl.md)).
Consider this example:

```c#
[Name("Self insured name validation")]
public class SelfInsuredNameValidationRule : Rule
{
    public override void Define()
    {
        Claim claim = default!;
        Patient patient = default!;

        When()
            .Match<Claim>(() => claim)
            .Match<Patient>(() => patient, p => p == claim.Patient,
                p => p.RelationshipToInsured == Relationship.Self)
            .Match<Insured>(i => i == claim.Insured,
                i => !Equals(patient.Name, i.Name));

        Then()
            .Do(ctx => ctx.Warning(claim, "Self insured name does not match"));
    }
}

public static class ContextExtensions
{
    public static void Warning(this IContext context, Claim claim, string message)
    {
        var warning = new ClaimAlert { Severity = 2, Claim = claim, RuleName = context.Rule.Name, Message = message };
        context.Insert(warning);
    }        
}
```

This rule matches a claim for a self-insured patient, and makes sure that the name of the patient and insured matches.
And if the names don't match, it creates a warning-level claim alert.

The rule would look much more readable if we could write it like this:

```c#
[Name("Self insured name validation")]
public class SelfInsuredNameValidationRule : Rule
{
    public override void Define()
    {
        Claim claim = default!;
        Patient patient = default!;

        When()
            .Claim(() => claim)
            .Patient(() => patient, p => p == claim.Patient, 
                p => p.RelationshipToInsured == Relationship.Self)
            .Insured(i => i == claim.Insured, 
                i => !Equals(patient.Name, i.Name));

        Then()
            .Warning(claim, "Self insured name does not match");
    }
}
```

And the good news is that this can be done, by defining DSL extension methods like this:

```c#
public static class DslExtensions
{
    public static ILeftHandSideExpression Claim(this ILeftHandSideExpression lhs, Expression<Func<Claim>> alias, params Expression<Func<Claim, bool>>[] conditions)
    {
        return lhs.Match(alias, conditions);
    }

    public static ILeftHandSideExpression Patient(this ILeftHandSideExpression lhs, Expression<Func<Patient>> alias, params Expression<Func<Patient, bool>>[] conditions)
    {
        return lhs.Match(alias, conditions);
    }

    public static ILeftHandSideExpression Insured(this ILeftHandSideExpression lhs, params Expression<Func<Insured, bool>>[] conditions)
    {
        return lhs.Match(conditions);
    }

    public static IRightHandSideExpression Warning(this IRightHandSideExpression rhs, Claim claim, string message)
    {
        return rhs.Do(ctx => ctx.Warning(claim, message));
    }
}
```