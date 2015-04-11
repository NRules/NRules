using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.ValidationRules
{
    [Name("Patient validation")]
    public class PatientValidationRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Match<Claim>(() => claim)
                .Or(x => x
                    .Not<Patient>(p => p == claim.Patient)
                    .Match<Patient>(p => p == claim.Patient, p => p.Name.IsEmpty && p.Address.IsEmpty));

            Then()
                .Do(ctx => ctx.Error(claim, "Patient information not provided"));
        }
    }
}