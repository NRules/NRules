using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.ValidationRules
{
    [Name("Insured validation")]
    public class InsuredValidationRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Match<Claim>(() => claim)
                .Or(x => x
                    .Not<Insured>(i => i == claim.Insured)
                    .Match<Insured>(i => i == claim.Insured, i => i.Name.IsEmpty && i.Address.IsEmpty));

            Then()
                .Do(ctx => ctx.Error(claim, "Insured information not provided"));
        }
    }
}