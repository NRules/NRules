using FluentNHibernate.Mapping;

namespace NRules.Samples.ClaimsExpert.Domain.Mappings
{
    public class PatientMap : ComponentMap<Patient>
    {
        public PatientMap()
        {
            Map(x => x.DateOfBirth, "Dob");
            Map(x => x.Sex, "Sex");
            Map(x => x.Phone);
            Map(x => x.RelationshipToInsured, "Relationship");
            Map(x => x.MaritalStatus);
            Map(x => x.EmploymentStatus);

            Component(x => x.Name);
            Component(x => x.Address).ColumnPrefix("PatientAddress");
        }
    }
}