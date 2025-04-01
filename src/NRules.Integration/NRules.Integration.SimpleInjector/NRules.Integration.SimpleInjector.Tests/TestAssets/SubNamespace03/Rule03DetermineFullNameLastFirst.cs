using NRules.Fluent.Dsl;

namespace NRules.Integration.SimpleInjector.Tests.TestAssets.SubNamespace03;

public class Rule03DetermineFullNameLastFirst : Rule
{
    public override void Define()
    {
        TestPerson fact1 = default!;

        When()
            .Match(() => fact1, p => p.FullName == null);

        Then()
            .Do(ctx => ctx.Update(ApplyFullName(fact1, $"{fact1.LastName}, {fact1.FirstName}")));
    }

    private TestPerson ApplyFullName(TestPerson person, string fullName)
    {
        person.FullName = fullName;
        return person;
    }
}
