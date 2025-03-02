using Domain;
using NRules.Fluent.Dsl;

namespace Rules;

public class YoungDriverSurchargeRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;

        When()
            .Match(() => quote, q => q.Driver.Age < 25);

        Then()
            .Do(ctx => quote.ApplySurcharge(100));
    }
}