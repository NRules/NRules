using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.ValidationRules
{
    [Name("Self insured name validation")]
    public class SelfInsuredNameValidationRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;
            Patient patient = null;

            When()
                .Claim(() => claim, c => c.Open)
                .Patient(() => patient, p => p == claim.Patient, 
                    p => p.RelationshipToInsured == Relationship.Self)
                .Insured(i => i == claim.Insured, 
                    i => !Equals(patient.Name, i.Name));

            Then()
                .Warning(claim, "Self insured name does not match");
        }
    }
}