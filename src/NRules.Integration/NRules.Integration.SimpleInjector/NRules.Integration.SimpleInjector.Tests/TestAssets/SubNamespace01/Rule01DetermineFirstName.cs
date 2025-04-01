using NRules.Fluent.Dsl;

namespace NRules.Integration.SimpleInjector.Tests.TestAssets.SubNamespace01;

public class Rule01DetermineFirstName : Rule
{
    public override void Define()
    {
        TestPerson fact1 = default!;

        When()
            .Match(() => fact1, p => p.FirstName == null);

        Then()
            .Do(ctx => ctx.Update(ApplyFirstName(fact1, "Brat")));
    }

    private TestPerson ApplyFirstName(TestPerson person, string firstName)
    {
        person.FirstName = firstName;
        return person;
    }
}
