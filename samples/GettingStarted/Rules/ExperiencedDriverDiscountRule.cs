using Domain;
using NRules.Fluent.Dsl;

namespace Rules;

public class ExperiencedDriverDiscountRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;

        When()
            .Match(() => quote, q => q.Driver.YearsOfExperience >= 5);

        Then()
            .Do(ctx => quote.ApplyDiscount(50));
    }
}