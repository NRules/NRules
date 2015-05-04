using FluentNHibernate.Mapping;

namespace NRules.Samples.ClaimsExpert.Domain.Mappings
{
    public class ClaimAlertMap : ClassMap<ClaimAlert>
    {
        public ClaimAlertMap()
        {
            Table("ClaimAlert");

            Id(x => x.Id).GeneratedBy.Identity();
            
            Map(x => x.Severity);
            Map(x => x.RuleName);
            Map(x => x.Message);

            References(x => x.Claim, "ClaimId").Fetch.Join();
        }
    }
}
