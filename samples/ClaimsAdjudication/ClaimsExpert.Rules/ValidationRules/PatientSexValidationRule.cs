using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.ValidationRules
{
    [Name("Patient sex validation")]
    public class PatientSexValidationRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Claim(() => claim, c => c.Open)
                .Patient(p => p == claim.Patient,
                    p => p.Sex == Sex.Unspecified);

            Then()
                .Error(claim, "Patient sex not specified");
        }
    }
}