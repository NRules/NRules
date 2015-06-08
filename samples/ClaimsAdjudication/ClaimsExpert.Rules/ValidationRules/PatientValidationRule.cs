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
                .Claim(() => claim, c => c.Open)
                .Or(x => x
                    .Not<Patient>(p => p == claim.Patient)
                    .Patient(p => p == claim.Patient, p => p.Name.IsEmpty && p.Address.IsEmpty));

            Then()
                .Error(claim, "Patient information not provided");
        }
    }
}