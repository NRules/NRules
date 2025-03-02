using Domain;
using NRules.Fluent.Dsl;

namespace Rules;

public class TrafficViolationSurchargeRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;
        IEnumerable<TrafficViolation> violations = default!;
        int totalViolations = 0;

        When()
            .Match(() => quote)
            .Query(() => violations, q => q
                .Match<TrafficViolation>()
                .Where(v => v.Driver == quote.Driver,
                    v => v.ViolationType != "Parking",
                    v => v.Date >= DateTime.Now.AddYears(-2))
                .Collect())
            .Let(() => totalViolations, () => violations.Count())
            .Having(() => totalViolations > 1);
        
        Then()
            .Do(ctx => quote.ApplySurcharge(20 * totalViolations));
    }
}