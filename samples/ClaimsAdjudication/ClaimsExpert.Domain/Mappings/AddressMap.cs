using FluentNHibernate.Mapping;

namespace NRules.Samples.ClaimsExpert.Domain.Mappings
{
    public class AddressMap : ComponentMap<Address>
    {
        public AddressMap()
        {
            Map(x => x.Line1);
            Map(x => x.Line2);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
        }
    }
}