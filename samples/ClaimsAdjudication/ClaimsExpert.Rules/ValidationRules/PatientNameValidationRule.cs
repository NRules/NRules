using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.ValidationRules
{
    [Name("Patient name validation")]
    public class PatientNameValidationRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Claim(() => claim, c => c.Open)
                .Patient(p => p == claim.Patient,
                    p => p.Name.FirstName == null || p.Name.LastName == null);

            Then()
                .Error(claim, "Patient name not fully specified");
        }
    }
}