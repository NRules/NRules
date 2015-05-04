using FluentNHibernate.Mapping;

namespace NRules.Samples.ClaimsExpert.Domain.Mappings
{
    public class InsuredMap : ComponentMap<Insured>
    {
        public InsuredMap()
        {
            Map(x => x.DateOfBirth, "Dob");
            Map(x => x.Sex, "Sex");
            Map(x => x.Phone);
            Map(x => x.EmployerName);
            Map(x => x.PolicyNumber);
            Map(x => x.PlanName);

            Component(x => x.Name);
            Component(x => x.Address).ColumnPrefix("InsuredAddress");
        }
    }
}