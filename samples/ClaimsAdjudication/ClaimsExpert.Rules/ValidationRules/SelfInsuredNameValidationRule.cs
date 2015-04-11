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
                .Match<Claim>(() => claim)
                .Match<Patient>(() => patient, p => p == claim.Patient, 
                    p => p.RelationshipToInsured == Relationship.Self)
                .Match<Insured>(i => i == claim.Insured, 
                    i => !Equals(patient.Name, i.Name));

            Then()
                .Do(ctx => ctx.Warning(claim, "Self insured name does not match"));
        }
    }
}