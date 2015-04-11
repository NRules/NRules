using FluentNHibernate.Mapping;

namespace NRules.Samples.ClaimsExpert.Domain.Mappings
{
    public class ClaimMap : ClassMap<Claim>
    {
        public ClaimMap()
        {
            Table("Claim");

            Id(x => x.Id);

            Map(x => x.ClaimType);
            Map(x => x.Status);

            Component(x => x.Patient).ColumnPrefix("Patient");
            Component(x => x.Insured).ColumnPrefix("Insured");

            HasMany(x => x.Alerts).KeyColumn("ClaimId")
                .Cascade.AllDeleteOrphan()
                .Inverse().Fetch.Join().Not.LazyLoad();
        }
    }
}