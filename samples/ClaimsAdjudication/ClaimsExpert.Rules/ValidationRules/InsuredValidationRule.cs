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
                .Claim(() => claim, c => c.Open)
                .Or(x => x
                    .Not<Insured>(i => i == claim.Insured)
                    .Insured(i => i == claim.Insured, i => i.Name.IsEmpty && i.Address.IsEmpty));

            Then()
                .Error(claim, "Insured information not provided");
        }
    }
}