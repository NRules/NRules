using NRules.Fluent.Dsl;

namespace NRules.Integration.SimpleInjector.Tests.TestAssets.SubNamespace02;

public class Rule02DetermineFullNameFirstLast : Rule
{
    public override void Define()
    {
        TestPerson fact1 = default!;

        When()
            .Match(() => fact1, p => p.FullName == null);

        Then()
            .Do(ctx => ctx.Update(ApplyFullName(fact1, $"{fact1.FirstName} {fact1.LastName}")));
    }

    private TestPerson ApplyFullName(TestPerson person, string fullName)
    {
        person.FullName = fullName;
        return person;
    }
}
