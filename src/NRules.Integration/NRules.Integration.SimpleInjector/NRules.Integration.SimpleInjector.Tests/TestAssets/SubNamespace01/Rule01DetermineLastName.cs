using NRules.Fluent.Dsl;

namespace NRules.Integration.SimpleInjector.Tests.TestAssets.SubNamespace01;

public class Rule01DetermineLastName : Rule
{
    public override void Define()
    {
        TestPerson fact1 = default!;

        When()
            .Match(() => fact1, p => p.LastName == null);

        Then()
            .Do(ctx => ctx.Update(ApplyLastName(fact1, "Pitt")));
    }

    private TestPerson ApplyLastName(TestPerson person, string lastName)
    {
        person.LastName = lastName;
        return person;
    }
}
